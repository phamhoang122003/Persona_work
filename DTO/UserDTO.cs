using Persona_work_management.Entities;

namespace Persona_work_management.DTO
{
	public class UserDTO
	{
		public int Id { get; set; }
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Role { get; set; } = null!;
		public string? AvatarUrl { get; set; } // URL ảnh đại diện
	}

}
