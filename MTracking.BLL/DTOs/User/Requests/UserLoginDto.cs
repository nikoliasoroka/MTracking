using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.User.Requests
{
    public class UserLoginDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
