using System;
using MTracking.BLL.DTOs.Description.Response;
using MTracking.BLL.DTOs.File.Response;
using MTracking.BLL.DTOs.Topic.Response;

namespace MTracking.BLL.DTOs.TimeLog.Response
{
    public class TimeLogResponseDto
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public int WorkTime { get; set; }
        public bool IsEditable { get; set; }
        public bool isSynchronize { get; set; }
        public int? CommitRecordId { get; set; }
        public FileResponseDto File { get; set; }
        public TopicResponseDto Topic { get; set; }
        public DescriptionResponseDto Description { get; set; }
    }
}