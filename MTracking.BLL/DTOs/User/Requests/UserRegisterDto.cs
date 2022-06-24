using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.User.Requests
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contains: upper case (A-Z), lower case (a-z), number (0-9)")]
        public string Password { get; set; }
    }
}
