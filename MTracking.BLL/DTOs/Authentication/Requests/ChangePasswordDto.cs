using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Authentication.Requests
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contains: upper case (A-Z), lower case (a-z), number (0-9)")]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match!")]
        public string ConfirmPassword { get; set; }
    }
}
