using Persona_work_management.Entities;

namespace Persona_work_management.DTO
{
	public class TaskDTO
	{
		public int Id { get; set; } // Khóa chính
		public string Title { get; set; } = null!;
		public string Description { get; set; } = null!;
		public DateTime DueDate { get; set; } // Hạn chót
		public string Priority { get; set; } = "Medium";  // Giá trị mặc định nếu không có giá trị trong request
		public string Status { get; set; } = "Pending";  // Giá trị mặc định nếu không có giá trị trong request
		public string Color { get; set; } = "Green";

		// Không đưa toàn bộ User vào để tránh vòng lặp
		public int UserId { get; set; } // Người sở hữu Task

		// Danh sách các thông báo liên quan
		public List<NotificationDTO> Notifications { get; set; } = new List<NotificationDTO>();
	}
}
