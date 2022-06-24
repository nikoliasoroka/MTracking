using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Reminder.Request
{
    public class ReminderCreateRequestDto
    {
        [Required]
        [Range(0, 23)]
        public int Hours { get; set; }
        [Required]
        [Range(0, 59)]
        public int Minutes { get; set; }
        public bool IsActive { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}
