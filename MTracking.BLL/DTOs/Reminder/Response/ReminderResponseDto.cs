using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Reminder.Response
{
    public class ReminderResponseDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(0, 23)]
        public int Hours { get; set; }
        [Required]
        [Range(0, 59)]
        public int Minutes { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool Monday { get; set; }
        [Required]
        public bool Tuesday { get; set; }
        [Required]
        public bool Wednesday { get; set; }
        [Required]
        public bool Thursday { get; set; }
        [Required]
        public bool Friday { get; set; }
        [Required]
        public bool Saturday { get; set; }
        [Required]
        public bool Sunday { get; set; }
    }
}
