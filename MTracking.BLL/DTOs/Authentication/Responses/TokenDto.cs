using System;
using System.ComponentModel.DataAnnotations;

namespace MTracking.BLL.DTOs.Authentication.Responses
{
    public class TokenDto
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public DateTime AccessTokenExpiredTime { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public DateTime RefreshTokenExpiredTime { get; set; }
    }
}
