using Persona_work_management.DTO;
using Persona_work_management.Entities;

namespace Persona_work_management.Service.Interfaces
{
	public interface ITasksService
	{
		Task<IEnumerable<TaskDTO>> GetTasks();
		Task<TaskDTO> GetTasksById(int id);
		Task<IEnumerable<TaskDTO>> GetAllTaskbyCustomerId(int customerId);
		
		Task UpdateTask(TaskDTO taskDTO, int id);
		Task DeleteTask(int id);

		Task<TaskDTO> CreateTask(TaskDTO taskDto);
	}
}
