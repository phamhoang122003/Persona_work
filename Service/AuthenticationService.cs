using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Persona_work_management.DAL;
using Persona_work_management.Entities;
using Persona_work_management.Service.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Persona_work_management.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ManagementDbContext _context;
        private readonly IConfiguration _configuration;
		// Inject IConfiguration qua constructor
		public AuthenticationService(IConfiguration configuration, ManagementDbContext context)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			_context = context ?? throw new ArgumentNullException(nameof(context), "DbContext is null.");
		}

		public async Task<Users?> AuthenticateAsync(string username, string password)
        {
            // Lấy user từ cơ sở dữ liệu
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

            // Kiểm tra nếu không tìm thấy user hoặc mật khẩu không đúng
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                TestPasswordVerification();

				return null; // Trả về null nếu không xác thực thành công
            }

            return user; // Trả về user nếu xác thực thành công
        }

        public string GenerateJwtToken(Users user)
        {
			// Đảm bảo rằng _configuration không null
			if (_configuration == null)
			{
				throw new InvalidOperationException("Configuration is not initialized.");
			}

			var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();

			if (jwtSettings == null)
			{
				throw new InvalidOperationException("JWT settings are missing in the configuration.");
			}

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),  // Người dùng (username)
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID của token
				new Claim(ClaimTypes.Role, user.Role.ToString())  // Thêm role vào claim
    };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwtSettings.Issuer,
				audience: jwtSettings.Audience,
				claims: claims,
				expires: DateTime.Now.AddHours(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);  // Trả về token dưới dạng chuỗi
		}


        private bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}
		public void TestPasswordVerification()
		{
			// Mã hóa mật khẩu
			string password = "123";
			string hashedPassword = HashPassword(password);
			Console.WriteLine($"Hashed Password: {hashedPassword}");

			// Xác minh mật khẩu
			bool isPasswordCorrect = VerifyPassword(password, hashedPassword);
			Console.WriteLine($"Password verification result: {isPasswordCorrect}");
		}
	}
}
