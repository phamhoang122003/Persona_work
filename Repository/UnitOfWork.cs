using Persona_work_management.DAL;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Generic;
using Persona_work_management.Repository.Interfaces;

namespace Persona_work_management.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ManagementDbContext _context;

		public UnitOfWork(ManagementDbContext context)
		{
			_context = context;
			// Khởi tạo các repository đặc thù
			UsersRepository = new UserRepository(_context);
			TasksRepository = new TasksRepository(_context);
			NotificationsRepository = new NotificationRepository(_context);
		}

		// Các repository đặc thù
		public IUsersRepository UsersRepository { get; }
		public ITasksRepository TasksRepository { get; }
		public INotificationsRepository NotificationsRepository { get; }

		// Lưu thay đổi vào cơ sở dữ liệu
		public async Task CompleteAsync()
		{
			await _context.SaveChangesAsync();
		}

		// Giải phóng tài nguyên
		public void Dispose()
		{
			_context.Dispose();
		}
	}

}
