using AutoMapper;
using Persona_work_management.DTO;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Interfaces;
using Persona_work_management.Service.Interfaces;

namespace Persona_work_management.Service
{
	public class NotificationService : INotificationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<NotificationDTO> CreateNotification(NotificationDTO notificationDTO)
		{
			var noti = _mapper.Map<Notification>(notificationDTO);
			await _unitOfWork.NotificationsRepository.AddAsync(noti);
			await _unitOfWork.CompleteAsync();
			return _mapper.Map<NotificationDTO>(noti);
		}

		public async Task DeleteNotification(int id)
		{
			var noti = await _unitOfWork.NotificationsRepository.GetByIdAsync(id);
			await _unitOfWork.NotificationsRepository.DeleteAsync(noti);
			await _unitOfWork.CompleteAsync();
		}

		public async Task<IEnumerable<NotificationDTO>> GetAllByTaskId(int id)
		{
			var notis =await _unitOfWork.NotificationsRepository.GetAllByTaskId(id);
			return _mapper.Map<IEnumerable<NotificationDTO>>(notis);
		}

		public async Task<IEnumerable<NotificationDTO>> GetNotification()
		{
			var notis = await _unitOfWork.NotificationsRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<NotificationDTO>>(notis);
		}

		public async Task<NotificationDTO> GetNotificationById(int id)
		{
			var noti = await _unitOfWork.NotificationsRepository.GetByIdAsync(id);
			return _mapper.Map<NotificationDTO>(noti);
		}

		public async Task UpdateNotification(NotificationDTO notificationDTO, int id)
		{
			var noti = await _unitOfWork.NotificationsRepository.GetByIdAsync(id);

			if (notificationDTO.NotificationTime != default(DateTime))
			{
				noti.NotificationTime = notificationDTO.NotificationTime;
			}

			noti.Message = notificationDTO.Message ?? noti.Message;
			await _unitOfWork.CompleteAsync();
		}
	}
}
