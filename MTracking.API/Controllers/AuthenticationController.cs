using System;
using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Authentication.Requests;
using MTracking.BLL.DTOs.Authentication.Responses;
using MTracking.BLL.DTOs.User.Requests;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        
        [Obsolete]
        [HttpPost("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserLoginResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Search(string name)
            => (await _authenticationService.SearchAsync(name)).ToActionResult();

        /// <summary>
        /// Register user into the system.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserLoginResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
            => (await _authenticationService.RegisterAsync(userDto)).ToActionResult();

        /// <summary>
        /// Login user into the system.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserLoginResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Login([FromBody]UserLoginDto userDto) 
            => (await _authenticationService.LoginAsync(userDto)).ToActionResult();

        /// <summary>
        /// Get a new access token by the refresh token.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken) 
            => (await _authenticationService.RefreshTokenAsync(refreshToken)).ToActionResult();

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="passwordDto"></param>
        /// <returns></returns>
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(UserLoginResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto passwordDto)
            => (await _authenticationService.ChangePasswordAsync(passwordDto)).ToActionResult();

        /// <summary>
        /// Forgot password, send verification code.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType( 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> ForgotPassword([Required] [EmailAddress] string email)
            => (await _authenticationService.SendVerificationCodeAsync(email)).ToActionResult();

        /// <summary>
        /// Set new password.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("set-password")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> SetNewPassword([FromBody] SetPasswordDto dto)
            => (await _authenticationService.SetNewPasswordAsync(dto)).ToActionResult();

        /// <summary>
        /// Check verification code.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("check-code")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> CheckCode([FromBody] CheckCodeDto dto)
            => (await _authenticationService.CheckVerificationCodeAsync(dto)).ToActionResult();
    }
}
