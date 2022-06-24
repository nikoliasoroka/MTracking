using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.File.Request
{
    public class FileCreateRequestDto
    {
        [Required]
        public string Name { get; set; }
    }
}
