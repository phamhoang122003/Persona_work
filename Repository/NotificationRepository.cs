using Microsoft.EntityFrameworkCore;
using Persona_work_management.DAL;
using Persona_work_management.Entities;
using Persona_work_management.Repository.Generic;
using Persona_work_management.Repository.Interfaces;

namespace Persona_work_management.Repository
{
	public class NotificationRepository : Repository<Notification>, INotificationsRepository
	{
		public NotificationRepository(ManagementDbContext context) : base(context)
		{
		}

		public async Task<IEnumerable<Notification>> GetAllByTaskId(int taskId)
		{	
			var notification = from n in _context.Notifications
							   where n.TaskId == taskId
							   select n;

			return await notification.ToListAsync();
		}
	}
}
