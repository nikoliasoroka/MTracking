using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Reminder.Request;
using MTracking.BLL.DTOs.Reminder.Response;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/reminder")]
    [ApiController]
    [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        /// <summary>
        /// Get reminder by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReminderResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetReminder([Required] int id)
            => (await _reminderService.GetByIdAsync(id)).ToActionResult();

        /// <summary>
        /// Get all reminders.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<ReminderResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetAll()
            => (await _reminderService.GetAllAsync()).ToActionResult();

        /// <summary>
        /// Create custom reminder.
        /// </summary>
        /// <param name="reminderDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ReminderResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> CreateReminder([FromBody] ReminderCreateRequestDto reminderDto)
            => (await _reminderService.CreateAsync(reminderDto)).ToActionResult();

        /// <summary>
        /// Update reminder.
        /// </summary>
        /// <param name="reminderDto"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ReminderResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> UpdateReminder([FromBody] ReminderUpdateRequestDto reminderDto)
            => (await _reminderService.UpdateAsync(reminderDto)).ToActionResult();

        /// <summary>
        /// Delete reminder by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ReminderResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> DeleteReminder([Required] int id)
            => (await _reminderService.DeleteAsync(id)).ToActionResult();
    }
}
