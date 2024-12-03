namespace Persona_work_management.DTO
{
	public class NotificationDTO
	{
		public int Id { get; set; } // Khóa chính
		public string Message { get; set; } = null!;
		public DateTime NotificationTime { get; set; } // Thời gian thông báo
		public bool IsSent { get; set; } // Trạng thái gửi thông báo

		public long Offset { get; set; } // Khoảng thời gian trước DueDate

		// Không đưa toàn bộ Task vào DTO để tránh vòng lặp phức tạp
		public int TaskId { get; set; } // Tham chiếu tới Task
	}
}
