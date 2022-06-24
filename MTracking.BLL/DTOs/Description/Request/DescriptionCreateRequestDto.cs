using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Description.Request
{
    public class DescriptionCreateRequestDto
    {
        [Required]
        public string Name { get; set; }
    }
}
