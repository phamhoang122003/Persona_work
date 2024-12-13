using Microsoft.EntityFrameworkCore;
using Persona_work_management.DAL;
using Persona_work_management.Service.Interfaces;
using Persona_work_management.Service;
using Persona_work_management.Repository.Interfaces;
using Persona_work_management.Repository;
using Persona_work_management.Configurations;
using Persona_work_management.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ManagementDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đảm bảo rằng IConfiguration được cấu hình chính xác
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); // Đăng ký IUnitOfWork
builder.Services.AddScoped<ITasksService, TasksService>(); // Đăng ký ITasksService
builder.Services.AddScoped<IUsersService, UsersService>(); // Đăng ký IUsersService
builder.Services.AddScoped<INotificationService, NotificationService>(); // Đăng ký INotificationService
																		 // Đảm bảo rằng bạn đã đăng ký dịch vụ IAuthenticationService trong DI container
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddAutoMapper(typeof(Program));



//email sending
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHostedService<NotificationBackgroundService>();

// Thêm dịch vụ vào container.
builder.Services.AddControllers();

// Đảm bảo rằng appsettings.json được nạp vào đúng
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
// Đọc cấu hình từ appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Cấu hình Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = jwtSettings.Issuer, // Thông tin phát hành token
			ValidateAudience = true,
			ValidAudience = jwtSettings.Audience, // Đối tượng nhận token
			ValidateLifetime = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)), // Secret key để ký token
			RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" // Chỉ định tên claim role đúng
		
	};
	});
// Cấu hình Authorization
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));  // Chỉ cho phép người dùng có role "Admin" truy cập
});


// Cấu hình dịch vụ CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigin", policy =>
	{
		policy.WithOrigins("http://localhost:5173") // Địa chỉ front-end
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});


var app = builder.Build();


// Sử dụng CORS
app.UseCors("AllowSpecificOrigin");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
