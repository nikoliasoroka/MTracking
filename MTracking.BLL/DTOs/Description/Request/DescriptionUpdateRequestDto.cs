using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Description.Request
{
    public class DescriptionUpdateRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
