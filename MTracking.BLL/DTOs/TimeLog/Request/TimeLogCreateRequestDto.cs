using System;
using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.TimeLog.Request
{
    public class TimeLogCreateRequestDto
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int WorkTime { get; set; }
        [Required]
        public int FileId { get; set; }
        public int? TopicId { get; set; }
        [Required]
        public int? DescriptionId { get; set; }
    }
}
