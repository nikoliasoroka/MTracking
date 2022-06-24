using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Topic.Request
{
    public class TopicCreateRequestDto
    {
        [Required] 
        public string Name { get; set; }
    }
}
