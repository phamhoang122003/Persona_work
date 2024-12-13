using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persona_work_management.DTO;
using Persona_work_management.Service;
using Persona_work_management.Service.Interfaces;

namespace Persona_work_management.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NotificationController : ControllerBase
	{
		private readonly INotificationService _notificationService;

		public NotificationController(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetAll()
		{
			var notifis = await _notificationService.GetNotification();
			return Ok(notifis);
		}
		[HttpGet("{id}")]
		public async Task<ActionResult<NotificationDTO>> GetOne(int id)
		{
			var notification = await _notificationService.GetNotificationById(id);
			if(notification == null)
			{
				return NotFound();
			}
			return Ok(notification);
		}
		[HttpGet("/task/{id}")]
		public async Task<ActionResult<NotificationDTO>> GetAllbyTaskId(int id)
		{
			var notification = await _notificationService.GetAllByTaskId(id);
			if (notification == null)
			{
				return NotFound();
			}
			return Ok(notification);
		}

		[HttpPost]
		public async Task<ActionResult<NotificationDTO>> Post([FromBody] NotificationDTO notification)
		{

			var newNoti = await _notificationService.CreateNotification(notification);
			return CreatedAtAction(nameof(GetOne), new { id = newNoti.Id }, newNoti);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<NotificationDTO>> Update([FromBody] NotificationDTO notification, int id) 
		{
			if(notification == null || id != notification.Id)
			{
				return BadRequest();
			}
			await _notificationService.UpdateNotification(notification, id);
			return Ok();
		}
		[HttpDelete("id")]
		public async Task<IActionResult> Delete(int id) 
		{
			await _notificationService.DeleteNotification(id);
			return Ok();
		}
	}
}
