using Persona_work_management.DAL;
using Persona_work_management.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persona_work_management.Entities;

namespace Persona_work_management.Service
{
	public class NotificationBackgroundService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;

		public NotificationBackgroundService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				await ProcessNotificationsAsync();
				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Kiểm tra mỗi phút
			}
		}

		private async Task ProcessNotificationsAsync()
		{
			Console.WriteLine("Processing notifications...");
			using var scope = _serviceProvider.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<ManagementDbContext>();
			var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

			var currentTime = DateTime.Now; // Lấy thời gian hiện tại
			var roundedCurrentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0); // Làm tròn đến phút

			var notificationsWithUserInfo = await dbContext.Notifications
				.Include(n => n.Task)  // Kết nối Notification với Task
				.ThenInclude(t => t.User)  // Kết nối Task với User
				.Where(n => !n.IsSent &&
							n.NotificationTime.Year == roundedCurrentTime.Year &&
							n.NotificationTime.Month == roundedCurrentTime.Month &&
							n.NotificationTime.Day == roundedCurrentTime.Day &&
							n.NotificationTime.Hour == roundedCurrentTime.Hour &&
							n.NotificationTime.Minute == roundedCurrentTime.Minute) // So sánh đến phút
				.Select(n => new
				{
					n.Id,
					n.IsSent,
					n.Message,             // Nội dung thông báo
					n.NotificationTime,    // Thời gian thông báo
					UserNamee = n.Task.User.UserName,  // Lấy tên người dùng
					UserEmail = n.Task.User.Email      // Lấy email người dùng
				})
				.ToListAsync();

			Console.WriteLine($"Found {notificationsWithUserInfo.Count} notifications to send.");
			foreach (var notification in notificationsWithUserInfo)
			{
				// Gửi email
				await emailService.SendEmailAsync(
					toEmail: notification.UserEmail, // Thay bằng email từ Notification
					subject: "Notification Alert",
					body: notification.Message
				);

				// Lấy thông báo thực tế từ dbContext và đánh dấu là đã gửi
				var notificationToUpdate = await dbContext.Notifications
														   .FirstOrDefaultAsync(n => n.Id == notification.Id);

				if (notificationToUpdate != null)
				{
					notificationToUpdate.IsSent = true; // Cập nhật trạng thái đã gửi
				}
			}

			await dbContext.SaveChangesAsync(); // Cập nhật trạng thái IsSent trong cơ sở dữ liệu
			Console.WriteLine("Notifications processed and saved.");
		}
	}
}
