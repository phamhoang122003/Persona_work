using Persona_work_management.Entities;

namespace Persona_work_management.DTO
{
	public class UserDTO
	{
		public int Id { get; set; } // Khóa chính
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string PasswordHash { get; set; } = null!;
		public string Role { get; set; } // Vai trò (User hoặc Admin)

		// Danh sách các Task của người dùng
		public List<TaskDTO> Tasks { get; set; } = new List<TaskDTO>();
	}
}
