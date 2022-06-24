using System.Collections.Generic;
using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Device.Request;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/device")]
    [ApiController]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        private readonly IFirebaseService _firebaseService;

        public DeviceController(IDeviceService deviceService, IFirebaseService firebaseService)
        {
            _deviceService = deviceService;
            _firebaseService = firebaseService;
        }

        /// <summary>
        /// Register unique device firebase token
        /// </summary>
        /// <param name="deviceDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Register([FromBody] DeviceDto deviceDto)
            => (await _deviceService.AddOrUpdateDevice(deviceDto)).ToActionResult();

        /// <summary>
        /// Logout device and clear firebase token.
        /// </summary>
        /// <param name="removeDeviceDto"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> RemoveDevice([FromBody] DeviceDto removeDeviceDto)
            => (await _deviceService.RemoveDevice(removeDeviceDto)).ToActionResult();

        /// <summary>
        /// Testing of firebase messages.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("test-firebase")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> TestFirebase([FromQuery] List<string> tokens, string title, string body)
        {
            var result = await _firebaseService.SendNotification(tokens, title, body);

            if (!result.Success)
                return BadRequest(result.ErrorInfo);

            return Ok(result);
        }
    }
}
