namespace Persona_work_management.DTO
{
	public class UserCreateDTO
	{
		public string UserName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Password { get; set; } = null!;
		public IFormFile? Avatar { get; set; } // File upload ảnh đại diện
	}
}
