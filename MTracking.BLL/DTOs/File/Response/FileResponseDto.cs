using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.File.Response
{
    public class FileResponseDto
    {
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string PortfolioNumber { get; set; }
        [Required] public bool IsActive { get; set; }
        [Required] public bool IsBillable { get; set; }

        public bool IsPinned { get; set; }
    }
}