namespace Persona_work_management.DTO
{
	public class UserUpdateDTO
	{
		public int Id { get; set; }
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string? Password { get; set; } // Mật khẩu mới (nếu có)
		public IFormFile? Avatar { get; set; } // File upload ảnh mới
	}
}
