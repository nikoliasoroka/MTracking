using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Timer.Response;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/timer")]
    [ApiController]
    [Authorize]
    public class TimerController : ControllerBase
    {
        private readonly ITimerService _timerService;

        public TimerController(ITimerService timerService)
        {
            _timerService = timerService;
        }

        /// <summary>
        /// Get timer in seconds
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(TimerResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Get()
            => (await _timerService.GetAsync()).ToActionResult();

        /// <summary>
        /// Start timer.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TimerResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> StartTimer()
            => (await _timerService.StartAsync()).ToActionResult();

        /// <summary>
        /// Stop timer.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(TimerResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> StopTimer()
            => (await _timerService.StopAsync()).ToActionResult();

        /// <summary>
        /// Continue timer.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPut("continue")]
        [ProducesResponseType(typeof(TimerResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> ContinueTimer()
            => (await _timerService.ContinueAsync()).ToActionResult();

        /// <summary>
        /// Reset timer.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(TimerResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> ResetTimer()
            => (await _timerService.ResetAsync()).ToActionResult();
    }
}
