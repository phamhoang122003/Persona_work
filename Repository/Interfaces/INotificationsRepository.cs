using Persona_work_management.Entities;
using Persona_work_management.Repository.Generic;

namespace Persona_work_management.Repository.Interfaces
{
	public interface INotificationsRepository : IRepository<Notification>
	{
		Task<IEnumerable<Notification>> GetAllByTaskId(int taskId);
	}
}
