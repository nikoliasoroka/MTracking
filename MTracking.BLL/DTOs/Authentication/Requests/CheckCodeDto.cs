using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Authentication.Requests
{
    public class CheckCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }
    }
}
