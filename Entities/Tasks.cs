namespace Persona_work_management.Entities
{
    public class Tasks
    {
        public int Id { get; set; } // Khóa chính
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime DueDate { get; set; } // Hạn chót
        public Priority Priority { get; set; } = Priority.Medium; // Mức độ ưu tiên: High, Medium, Low
        public Status Status { get; set; } = Status.Pending; // Trạng thái: Pending, InProgress, Completed

        public LabelColor Color { get; set; } = LabelColor.Green; 

        // Khóa ngoại liên kết đến User
        public int UserId { get; set; }
        public Users User { get; set; } = null!;


        // Quan hệ 1-N: Một Task có nhiều Notification
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
