using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Topic.Response
{
    public class TopicResponseDto
    {
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        public bool IsCustom { get; set; }
    }
}