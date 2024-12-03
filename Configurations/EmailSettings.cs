namespace Persona_work_management.Configurations
{
	public class EmailSettings
	{
		public string SmtpServer { get; set; } = null!; // Máy chủ SMTP
		public int Port { get; set; } // Cổng SMTP
		public string SenderEmail { get; set; } = null!; // Email người gửi
		public string SenderPassword { get; set; } = null!; // Mật khẩu
	}
}
