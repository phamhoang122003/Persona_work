using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persona_work_management.DAL;
using Persona_work_management.DTO;
using Persona_work_management.Service.Interfaces;
using Persona_work_management.Repository.Interfaces;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoMapper;

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
		private readonly IMapper _mapper;

		public UsersController(IUsersService usersService, ManagementDbContext context, IEmailService emailService, IUnitOfWork unitOfWork, IMapper mapper)
		{
			_usersService = usersService;
			_context = context;
			_emailService = emailService;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		// Lấy tất cả người dùng (Admin chỉ có thể truy cập)
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUser()
		{
			var users = await _usersService.GetUser();
			return Ok(users);
		}

		// Lấy thông tin người dùng theo ID
		[HttpGet("{id}")]
		public async Task<ActionResult<UserDTO>> GetUser(int id)
		{
			var user = await _usersService.GetUserById(id);
			return Ok(user);
		}

		// Tạo người dùng mới
		[HttpPost]
		public async Task<ActionResult<UserDTO>> CreateUser([FromForm] UserCreateDTO userCreateDto)
		{
			var user = await _usersService.CreateUser(userCreateDto);
			var userDto = _mapper.Map<UserDTO>(user);
			return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
		}

		// Cập nhật người dùng
		[HttpPut("{id}")]
		public async Task<ActionResult<UserDTO>> UpdateUser([FromForm] UserUpdateDTO userDTO, int id)
		{
			if (userDTO == null || userDTO.Id != id)
			{
				return BadRequest();
			}
			await _usersService.UpdateUser(userDTO, id);
			return Ok();
		}	

		// Quên mật khẩu và gửi email reset
		[HttpGet("forgot_password/{email}")]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

			if (user == null)
			{
				return Ok(new { message = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn khôi phục mật khẩu." });
			}

			// Xóa token cũ nếu có
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
				// Lưu thông tin token vào DB
				await _context.SaveChangesAsync();

				// Tạo URL reset mật khẩu
				var resetUrl = $"http://localhost:5173/reset-password/{user.Id}/{resetToken}";

				// Gửi email với link reset mật khẩu
				await _emailService.SendEmailAsync(email, "Khôi phục mật khẩu",
					$"Nhấp vào liên kết sau để khôi phục mật khẩu: <a href='{resetUrl}'>{resetUrl}</a>. Liên kết này có hiệu lực trong vòng 10 phút.");
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể gửi email hoặc lưu thay đổi." });
			}

			return Ok(new { message = "Nếu email tồn tại trong hệ thống, chúng tôi đã gửi hướng dẫn khôi phục mật khẩu." });
		}

		// Tạo mã reset mật khẩu ngẫu nhiên (6 chữ số)
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

		// Reset mật khẩu bằng token
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
