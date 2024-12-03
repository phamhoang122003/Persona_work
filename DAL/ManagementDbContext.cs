using Microsoft.EntityFrameworkCore;
using Persona_work_management.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection.Emit;

namespace Persona_work_management.DAL
{
	public class ManagementDbContext : DbContext
	{
		// Constructor nhận DbContextOptions để cấu hình
		public ManagementDbContext(DbContextOptions<ManagementDbContext> options)
			: base(options)
		{
		}

		// DbSet cho các entity
		public DbSet<Users> Users { get; set; }
		public DbSet<Tasks> Tasks { get; set; }
		public DbSet<Notification> Notifications { get; set; }

		// Ghi đè phương thức OnModelCreating để cấu hình
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			// Cấu hình Priority (enum) trong Tasks lưu dưới dạng chuỗi
			modelBuilder.Entity<Tasks>()
				.Property(t => t.Priority)
				.HasConversion<string>();

			// Cấu hình Status (enum) trong Tasks lưu dưới dạng chuỗi
			modelBuilder.Entity<Tasks>()
				.Property(t => t.Status)
				.HasConversion<string>();

			// Cấu hình Color (enum) trong Labels lưu dưới dạng chuỗi
			modelBuilder.Entity<Tasks>()
				.Property(l => l.Color)
				.HasConversion<string>();

			// Cấu hình Role (enum) trong Users lưu dưới dạng chuỗi
			modelBuilder.Entity<Users>()
				.Property(u => u.Role)
				.HasConversion<string>();
			modelBuilder.Entity<Notification>()
				.Property(n => n.Offset)
				.HasColumnType("bigint"); // Lưu dưới dạng bigint
			base.OnModelCreating(modelBuilder);
		}
	}
}
