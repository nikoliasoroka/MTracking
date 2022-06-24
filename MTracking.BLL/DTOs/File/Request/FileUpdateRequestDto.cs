using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.File.Request
{
    public class FileUpdateRequestDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
