using System.Threading.Tasks;

namespace Persona_work_management.Entities
{
    public class Notification
    {
        public int Id { get; set; } // Khóa chính
        public string Message { get; set; } = null!;
        public DateTime NotificationTime { get; set; } // Thời gian thông báo
        public bool IsSent { get; set; } = false; // Trạng thái gửi thông báo

		public long Offset { get; set; } // Khoảng thời gian trước DueDate

		// Khóa ngoại liên kết đến Task
		public int TaskId { get; set; }
        public Tasks Task { get; set; } = null!;
    }
}
