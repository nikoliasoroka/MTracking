using System.ComponentModel.DataAnnotations;
using MTracking.BLL.DTOs.User.Responses;

namespace MTracking.BLL.DTOs.Authentication.Responses
{
    public class UserLoginResponseDto
    {
        [Required]
        public TokenDto Token { get; set; }

        [Required]
        public UserResponseDto User { get; set; }
    }
}
