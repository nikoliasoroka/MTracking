using System;
using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.TimeLog.Request
{
    public class TimeLogUpdateRequestDto
    {
        [Required] 
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int WorkTime { get; set; }
        public int FileId { get; set; }
        public int? TopicId { get; set; }
        public int? DescriptionId { get; set; }
    }
}
