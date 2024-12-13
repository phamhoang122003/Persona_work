using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persona_work_management.Migrations
{
	/// <inheritdoc />
	public partial class AllowNullResetPasswordFields : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Thay đổi cột ResetPasswordToken để cho phép NULL
			migrationBuilder.AlterColumn<string>(
				name: "ResetPasswordToken",
				table: "Users",
				type: "nvarchar(255)",
				nullable: true,  // Cho phép giá trị NULL
				oldClrType: typeof(string),
				oldType: "nvarchar(255)",
				oldNullable: false);

			// Thay đổi cột ResetPasswordExpiry để cho phép NULL
			migrationBuilder.AlterColumn<DateTime>(
				name: "ResetPasswordExpiry",
				table: "Users",
				type: "datetime2",
				nullable: true,  // Cho phép giá trị NULL
				oldClrType: typeof(DateTime),
				oldType: "datetime2",
				oldNullable: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// Nếu cần rollback, khôi phục các trường này về không cho phép NULL
			migrationBuilder.AlterColumn<string>(
				name: "ResetPasswordToken",
				table: "Users",
				type: "nvarchar(255)",
				nullable: false,  // Không cho phép NULL
				oldClrType: typeof(string),
				oldType: "nvarchar(255)",
				oldNullable: true);

			migrationBuilder.AlterColumn<DateTime>(
				name: "ResetPasswordExpiry",
				table: "Users",
				type: "datetime2",
				nullable: false,  // Không cho phép NULL
				oldClrType: typeof(DateTime),
				oldType: "datetime2",
				oldNullable: true);
		}
	}

}
