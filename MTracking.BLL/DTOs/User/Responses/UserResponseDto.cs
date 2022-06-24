using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.User.Responses
{
    public class UserResponseDto
    {
        [Required]
        public int Id { get; set; }
        
        public string HebrewName { get; set; }
        
        public string EnglishName { get; set; }

        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        public bool IsPasswordChanged { get; set; }

    }
}
