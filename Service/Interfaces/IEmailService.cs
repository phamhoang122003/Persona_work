namespace Persona_work_management.Service.Interfaces
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string body);
	}
}
