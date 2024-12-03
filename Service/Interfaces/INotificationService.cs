using Persona_work_management.DTO;

namespace Persona_work_management.Service.Interfaces
{
	public interface INotificationService
	{
		Task<IEnumerable<NotificationDTO>> GetNotification();
		Task<NotificationDTO> GetNotificationById(int id);

		Task UpdateNotification(NotificationDTO notificationDTO, int id);
		Task DeleteNotification(int id);

		Task<NotificationDTO> CreateNotification(NotificationDTO notificationDTO);

		Task<IEnumerable<NotificationDTO>> GetAllByTaskId(int id);
	}
}
