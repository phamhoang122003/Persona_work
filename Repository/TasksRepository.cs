using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persona_work_management.DAL;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Generic;
using Persona_work_management.Repository.Interfaces;

namespace Persona_work_management.Repository
{
	public class TasksRepository : Repository<Tasks>, ITasksRepository
	{
		public TasksRepository(ManagementDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Tasks>> GetAllTasksByCustomerIds(int idCustomer)
		{
			var task = from t in _context.Tasks
					   where t.UserId == idCustomer
					   select t;
			return await task.ToListAsync();
		}
	}
}
