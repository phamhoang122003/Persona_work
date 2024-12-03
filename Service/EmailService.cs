using Microsoft.Extensions.Options;
using MimeKit;
using Persona_work_management.Configurations;
using Persona_work_management.Service.Interfaces;
using MailKit.Net.Smtp; // Đảm bảo bạn đang dùng MailKit


namespace Persona_work_management.Service
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _emailSettings;

		public EmailService(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body)
		{
			try
			{
				Console.WriteLine($"Sending email to: {toEmail}");

				var emailMessage = new MimeMessage();
				emailMessage.From.Add(new MailboxAddress("Sender", _emailSettings.SenderEmail));
				emailMessage.To.Add(new MailboxAddress("Recipient", toEmail));
				emailMessage.Subject = subject;

				var bodyBuilder = new BodyBuilder
				{
					HtmlBody = body
				};
				emailMessage.Body = bodyBuilder.ToMessageBody();

				using var smtpClient = new SmtpClient();
				Console.WriteLine("Connecting to SMTP server...");
				await smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
				await smtpClient.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
				Console.WriteLine("Sending email...");
				await smtpClient.SendAsync(emailMessage);
				Console.WriteLine("Email sent successfully.");
			}
			catch (Exception ex)
			{
				// Log lỗi nếu có
				Console.WriteLine($"Error sending email: {ex.Message}");
			}
		}



	}

}
