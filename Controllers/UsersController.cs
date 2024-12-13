using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persona_work_management.DAL;
using Persona_work_management.DTO;
using Persona_work_management.Repository.Interfaces;
using Persona_work_management.Service.Interfaces;
using System.Net.Mail;
using System.Security.Cryptography;

namespace Persona_work_management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUsersService _usersService;
		private readonly ManagementDbContext _context;
		private readonly IEmailService _emailService;
		private readonly IUnitOfWork _unitOfWork;

		public UsersController(IUsersService usersService, ManagementDbContext context, IEmailService emailService, IUnitOfWork unitOfWork)
		{
			_usersService = usersService;
			_context = context;
			_emailService = emailService;
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUser()
		{
			var users = await _usersService.GetUser();
			return Ok(users);
		}
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<UserDTO>> GetUser(int id) 
		{
			var user = await _usersService.GetUserById(id);
			return Ok(user);
		}
		[HttpPost]
		public async Task<ActionResult<UserDTO>> CreateUser(UserDTO userDTO) 
		{
			var user = await _usersService.CreateUser(userDTO);
			return CreatedAtAction(nameof(GetUser),new {id = user.Id}, user);
		}
		[HttpPut("{id}")]
		public async Task<ActionResult<UserDTO>> UpdateUser(UserDTO userDTO,int id)
		{
			if (userDTO == null|| userDTO.Id != id )
			{
				return BadRequest();
			}
			await _usersService.UpdateUser(userDTO,id);
			return Ok();
		}
		// send email for forgot password

		//	[HttpGet("forgot_password/{email}")]
		//	public async Task<IActionResult> ForgotPassword(string email)
		//	{
		//	var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

		//	if (user == null)
		//	{
		// Nếu không tìm thấy user, tránh lộ thông tin. Cung cấp thông báo chung.
		//		return Ok(new { message = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn khôi phục mật khẩu." });
		//	}

		// Kiểm tra và xoá token cũ nếu có
		//if (!string.IsNullOrEmpty(user.ResetPasswordToken))
		////	{
		//		user.ResetPasswordToken = null;
		//		user.ResetPasswordExpiry = null;
		//	}

		// 1. Tạo mã token 6 chữ số ngẫu nhiên
		//	var resetToken = GenerateRandomToken();
		//	user.ResetPasswordToken = resetToken;
		//user.ResetPasswordExpiry = DateTime.UtcNow.AddMinutes(10); // Thời gian hết hạn token, ví dụ là 10 phút

		//	try
		//{
		// 2. Cập nhật thông tin người dùng với token mới
		//	await _context.SaveChangesAsync();

		// 3. Gửi email chứa token
		//	await _emailService.SendEmailAsync(email, "Mã xác thực đăng nhập",
		//		$"Mã xác thực của bạn là: <b>{resetToken}</b>. Mã này có hiệu lực trong vòng 10 phút.");
		//	}
		//	catch (Exception ex)
		//	{
		// Ghi log lỗi nếu không gửi được email hoặc lưu thay đổi thất bại
		//	return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể gửi email hoặc lưu thay đổi." });
		//}

		// Trả về thông báo chung
		//	return Ok(new { message = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi mã xác thực đăng nhập." });
		//}


		[HttpGet("forgot_password/{email}")]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null)
			{
				// Tránh lộ thông tin về user.
				return Ok(new { message = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn khôi phục mật khẩu." });
			}

			// Xoá token cũ nếu có
			if (!string.IsNullOrEmpty(user.ResetPasswordToken))
			{
				user.ResetPasswordToken = null;
				user.ResetPasswordExpiry = null;
			}

			// Tạo mã token ngẫu nhiên
			var resetToken = GenerateRandomToken();
			user.ResetPasswordToken = resetToken;
			user.ResetPasswordExpiry = DateTime.UtcNow.AddMinutes(10); // Token có hiệu lực 10 phút

			try
			{
				// Lưu token vào cơ sở dữ liệu
				await _context.SaveChangesAsync();

				// Tạo URL reset mật khẩu (định hướng theo frontend)
				var resetUrl = $"http://localhost:5173/reset-password/{user.Id}/{resetToken}";

				// Gửi email với đường dẫn reset mật khẩu
				await _emailService.SendEmailAsync(email, "Khôi phục mật khẩu",
					$"Nhấp vào liên kết sau để khôi phục mật khẩu: <a href='{resetUrl}'>{resetUrl}</a>. Liên kết này có hiệu lực trong vòng 10 phút.");
			}
			catch (Exception ex)
			{
				// Ghi log lỗi nếu xảy ra
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể gửi email hoặc lưu thay đổi." });
			}

			// Trả về thông báo chung
			return Ok(new { message = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn khôi phục mật khẩu." });
		}



		private string GenerateRandomToken()
		{
			using (var rng = new RNGCryptoServiceProvider())
			{
				byte[] tokenData = new byte[6];  // Mảng byte 6 phần tử
				rng.GetBytes(tokenData);
				int token = BitConverter.ToInt32(tokenData, 0) & 0xFFFFFF;  // Lấy 6 chữ số từ mảng byte
				return token.ToString("D6"); // Đảm bảo token có 6 chữ số
			}
		}


		[HttpPut("reset-password/{userId}/{token}")]
		public async Task<IActionResult> ResetPassword(int userId, string token, [FromBody] ResetPasswordModel request)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u =>
				u.Id == userId &&
				u.ResetPasswordToken == token &&
				u.ResetPasswordExpiry > DateTime.UtcNow);

			if (user == null)
			{
				return BadRequest(new { message = "Token không hợp lệ hoặc đã hết hạn." });
			}

			// Băm mật khẩu mới và lưu thay đổi
			user.PasswordHash = _usersService.HashPassword(request.NewPassword);
			user.ResetPasswordToken = null;
			user.ResetPasswordExpiry = null;

			await _unitOfWork.UsersRepository.UpdateAsync(user);
			await _context.SaveChangesAsync();

			return Ok(new { message = "Mật khẩu đã được đặt lại thành công." });
		}




	}
}
