using AutoMapper;

using Persona_work_management.DTO;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Interfaces;
using Persona_work_management.Service.Interfaces;

namespace Persona_work_management.Service
{
	public class TasksService : ITasksService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _appMapper;
		private readonly INotificationService _notificationService;
		public TasksService(IUnitOfWork unitOfWork, IMapper appMapper, INotificationService notificationService)
		{
			_unitOfWork = unitOfWork;
			_appMapper = appMapper;
			_notificationService = notificationService;
		}

		public async Task<TaskDTO> CreateTask(TaskDTO taskDto)
		{
			// Ánh xạ từ TaskDTO sang Task entity
			var task = _appMapper.Map<Tasks>(taskDto);

			// Thêm task vào cơ sở dữ liệu bất đồng bộ và đợi kết quả
			await _unitOfWork.TasksRepository.AddAsync(task);
			await _unitOfWork.CompleteAsync();
			// Ánh xạ từ Task entity sang TaskDTO và trả về
			return _appMapper.Map<TaskDTO>(task);
		}

		public async Task DeleteTask(int id)
		{
			var task = await _unitOfWork.TasksRepository.GetByIdAsync(id);
			var notifications = await _unitOfWork.NotificationsRepository.GetAllByTaskId(id);
			foreach (var notification in notifications)
			{
				await _unitOfWork.NotificationsRepository.DeleteAsync(notification);
			}
			if (task != null)
			{
				await _unitOfWork.TasksRepository.DeleteAsync(task);
			}
			await _unitOfWork.CompleteAsync();
		}

		public async Task<IEnumerable<TaskDTO>> GetAllTaskbyCustomerId(int customerId)
		{
			var tasks = await _unitOfWork.TasksRepository.GetAllTasksByCustomerIds(customerId);

			return _appMapper.Map<IEnumerable<TaskDTO>>(tasks);
		}

		public async Task<IEnumerable<TaskDTO>> GetTasks()
		{
			var task = await _unitOfWork.TasksRepository.GetAllAsync();
			return _appMapper.Map<IEnumerable<TaskDTO>>(task);
		}

		public async Task<TaskDTO> GetTasksById(int id)
		{
			var task = await _unitOfWork.TasksRepository.GetByIdAsync(id);
			return _appMapper.Map<TaskDTO>(task);
		}

		public async Task UpdateTask(TaskDTO taskDTO, int id)
		{
			// Lấy Task từ cơ sở dữ liệu theo ID
			var task = await _unitOfWork.TasksRepository.GetByIdAsync(id);

			if (task == null)
			{
				throw new Exception("Task not found");
			}

			// So sánh và cập nhật các thuộc tính từ taskDTO vào task entity
			task.Title = taskDTO.Title ?? task.Title;  // Nếu taskDTO.Title không null, cập nhật, nếu không giữ nguyên giá trị cũ
			task.Description = taskDTO.Description ?? task.Description;
			task.DueDate = taskDTO.DueDate != default ? taskDTO.DueDate : task.DueDate;  // Cập nhật nếu DueDate có giá trị mới
			task.UserId = taskDTO.UserId != 0 ? taskDTO.UserId : task.UserId;  // Cập nhật UserId nếu có

			// Chuyển đổi từ string sang enum nếu có giá trị mới trong TaskDTO
			task.Priority = !string.IsNullOrEmpty(taskDTO.Priority) ? Enum.Parse<Priority>(taskDTO.Priority) : task.Priority;
			task.Status = !string.IsNullOrEmpty(taskDTO.Status) ? Enum.Parse<Status>(taskDTO.Status) : task.Status;
			task.Color = !string.IsNullOrEmpty(taskDTO.Color) ? Enum.Parse<LabelColor>(taskDTO.Color) : task.Color;

			var notis = await _notificationService.GetAllByTaskId(id);
			foreach (var item in notis)
			{
				item.NotificationTime = task.DueDate - TimeSpan.FromTicks(item.Offset);
			}
			// Lưu các thay đổi vào cơ sở dữ liệu
			await _unitOfWork.CompleteAsync();
		}
	}
}
