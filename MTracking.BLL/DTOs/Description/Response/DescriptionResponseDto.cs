using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Description.Response
{
    public class DescriptionResponseDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsCustom { get; set; }
    }
}
