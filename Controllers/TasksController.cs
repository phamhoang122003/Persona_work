using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persona_work_management.DTO;
using Persona_work_management.Entities;
using Persona_work_management.Service.Interfaces;
using System.Threading.Tasks;

namespace Persona_work_management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TasksController : ControllerBase
	{
		private readonly ITasksService _tasksService;
		private readonly INotificationService _notificationService;

		public TasksController(ITasksService tasksService, INotificationService notificationService)
		{
			_tasksService = tasksService;
			_notificationService = notificationService;
		}

		[HttpGet]
		[Authorize(Roles ="Admin")]
		public async Task<ActionResult<IEnumerable<TaskDTO>>> GetAllTasks()
		{
			var tasks = await _tasksService.GetTasks();
			return Ok(tasks);
		}

		[HttpGet("/customer/{id}")]
		public async Task<ActionResult<IEnumerable<TaskDTO>>> GetAllTasksByCustomerId(int id)
		{
			var tasks = await _tasksService.GetAllTaskbyCustomerId(id);
			if(tasks == null)
			{
				return NotFound();
			}
			return Ok(tasks);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<TaskDTO>> GetTasksById(int id)
		{
			var tasks = await _tasksService.GetTasksById(id);
			if (tasks == null)
			{
				return NotFound();
			}
			return Ok(tasks);
		}

		[HttpPost]
		public async Task<ActionResult<TaskDTO>> PostTask([FromBody] TaskDTO taskDTO)
		{
			var newTask = await _tasksService.CreateTask(taskDTO);

			NotificationDTO notificationDTO = new NotificationDTO();
			notificationDTO.TaskId = newTask.Id;
			notificationDTO.Offset = TimeSpan.FromDays(1).Ticks;
			notificationDTO.NotificationTime = newTask.DueDate - TimeSpan.FromTicks(notificationDTO.Offset);
			notificationDTO.Message = "Task " + newTask.Title + " is due in 1 day.";
			await _notificationService.CreateNotification(notificationDTO);


			NotificationDTO notificationDTO1 = new NotificationDTO();
			notificationDTO1.TaskId = newTask.Id;
			notificationDTO1.Offset = TimeSpan.FromMinutes(5).Ticks;
			notificationDTO1.NotificationTime = newTask.DueDate - TimeSpan.FromTicks(notificationDTO1.Offset);
			notificationDTO1.Message = "Task " + newTask.Title + " is due in 5 minutes.";
			await _notificationService.CreateNotification(notificationDTO1);


			return CreatedAtAction(nameof(GetTasksById), new { id = newTask.Id }, newTask) ;
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<TaskDTO>> PustTask([FromBody] TaskDTO taskDTO,int id)
		{
			if(taskDTO == null || id != taskDTO.Id)
			{
				return BadRequest();
			}
			var notis = await _notificationService.GetAllByTaskId(id);
			foreach (var item in notis)
			{
				// Cập nhật thời gian thông báo

				item.NotificationTime = taskDTO.DueDate - TimeSpan.FromTicks(item.Offset);
				await _notificationService.UpdateNotification(item,item.Id);
				
			}
			await _tasksService.UpdateTask(taskDTO,id);


			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTasks(int id)
		{
			await _tasksService.DeleteTask(id);
			return NoContent();
		}
	}		
}
