using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Check;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/check")]
    [ApiController]
    public class CheckController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public CheckController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Check Is Sign Up Needed
        /// </summary>
        /// <returns></returns>
        [HttpGet("is-sign-up-needed")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CheckSignUpResponse), 200)]
        public async Task<IActionResult> IsSignUpNeeded()
        {
            return Ok(new CheckSignUpResponse{IsNeeded = true});
        }

        /// <summary>
        /// Testing email sending
        /// </summary>
        /// <returns></returns>
        [HttpPost("test")]
        [AllowAnonymous]
        public async Task<IActionResult> SendEmail([Required] string email)
        {
            var message = new EmailMessage(new string[] { email }, "Test email", $"Test email to {email}");

            var sendEmailResult = await _emailService.SendEmailAsync(new EmailMessage(new string[] { email }, "Test email", $"Test email to {email}"));

            if (!sendEmailResult.Success)
                return BadRequest(sendEmailResult.ErrorInfo.Exception.Message);

            return Ok($"Email was sent to {email} at {DateTime.Now}");
        }
    }
}
