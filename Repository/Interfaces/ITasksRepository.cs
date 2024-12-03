
using Persona_work_management.Entities;
using Persona_work_management.Repository.Generic;

namespace Persona_work_management.Repository.Interfaces
{
	public interface ITasksRepository : IRepository<Tasks>
	{
		Task<IEnumerable<Tasks>> GetAllTasksByCustomerIds(int idCustomer);
	}
}
