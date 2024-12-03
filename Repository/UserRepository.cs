using Microsoft.EntityFrameworkCore;
using Persona_work_management.DAL;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Generic;
using Persona_work_management.Repository.Interfaces;

namespace Persona_work_management.Repository
{
	public class UserRepository : Repository<Users>,IUsersRepository
	{
		public UserRepository(ManagementDbContext context) : base(context)
		{
		}
	}
}
