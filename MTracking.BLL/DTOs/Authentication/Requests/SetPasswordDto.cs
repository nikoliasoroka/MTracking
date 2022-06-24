using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Authentication.Requests
{
    public class SetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }

        [Required]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contains: upper case (A-Z), lower case (a-z), number (0-9)")]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match!")]
        public string ConfirmPassword { get; set; }
    }
}
