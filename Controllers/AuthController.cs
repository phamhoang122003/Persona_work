using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persona_work_management.Service.Interfaces;
using Persona_work_management.DTO;

namespace Persona_work_management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthenticationService _authService;

		public AuthController(IAuthenticationService authService)
		{
			_authService = authService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			// Xác thực thông tin đăng nhập
			var user = await _authService.AuthenticateAsync(request.Username, request.Password);

			// Nếu không tìm thấy user hoặc mật khẩu sai, trả về Unauthorized
			if (user == null)
			{
				return Unauthorized("Invalid username or password");
			}

			// Sinh JWT token sau khi xác thực thành công
			var token = _authService.GenerateJwtToken(user);

			// Trả về token cùng với thông tin người dùng
			return Ok(new { Token = token });
		}
	}
}
