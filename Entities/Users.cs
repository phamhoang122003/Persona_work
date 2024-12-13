namespace Persona_work_management.Entities
{
    public class Users
    {
        public int Id { get; set; } // Khóa chính
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public Role Role { get; set; } = Role.User; // Vai trò (User hoặc Admin)
		public string? ResetPasswordToken { get; set; } // Token khôi phục mật khẩu
		public DateTime? ResetPasswordExpiry { get; set; } // Thời gian hết hạn của token

		// Quan hệ 1-N: Một User có nhiều Task
		public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    }
}
