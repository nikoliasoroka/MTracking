using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Topic.Request
{
    public class TopicUpdateRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
