using System;
using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.TimeLog.Request;
using MTracking.BLL.DTOs.TimeLog.Response;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/timelog")]
    [ApiController]
    [Authorize]
    public class TimeLogController : ControllerBase
    {
        private readonly ITimeLogService _timeLogService;

        public TimeLogController(ITimeLogService timeLogService)
        {
            _timeLogService = timeLogService;
        }

        /// <summary>
        /// Get all TimeLogs by User (logging history).
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<TimeLogResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetAllTimeLogs([FromQuery] PaginationParams paginationParams)
            => (await _timeLogService.GetAllAsync(paginationParams)).ToActionResult();

        /// <summary>
        /// Get daily TimeLogs by User.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("daily")]
        [ProducesResponseType(typeof(PagedResult<TimeLogResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetDailyTimeLogs([FromQuery] PaginationParams paginationParams)
            => (await _timeLogService.GetDailyAsync(paginationParams)).ToActionResult();

        /// <summary>
        /// Get monthly TimeLogs by User.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("monthly")]
        [ProducesResponseType(typeof(PagedResult<TimeLogResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetMonthlyTimeLogs([FromQuery] PaginationParams paginationParams)
            => (await _timeLogService.GetMonthlyAsync(paginationParams)).ToActionResult();

        /// <summary>
        /// Get TimeLog by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TimeLogResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetTimeLog([Required] int id)
            => (await _timeLogService.GetByIdAsync(id)).ToActionResult();

        /// <summary>
        /// Search TimeLogs by Topic's name, Description's name, File's name.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<TimeLogResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Search([FromQuery] PaginationParams paginationParams,
            [FromQuery] DateTime? DateFrom, [FromQuery] DateTime? DateTo, [FromQuery] string query = "")
            => (await _timeLogService.SearchAsync(paginationParams, query, DateFrom, DateTo)).ToActionResult();

        /// <summary>
        /// Search monthly TimeLogs by Topic's name, Description's name, File's name.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("search-monthly")]
        [ProducesResponseType(typeof(PagedResult<TimeLogResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> SearchMonthly([FromQuery] PaginationParams paginationParams,
            [FromQuery] string query = "")
            => (await _timeLogService.SearchMonthlyAsync(paginationParams, query)).ToActionResult();

        /// <summary>
        /// Get TimeLog daily and monthly statistic (app home screen).
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("statistic")]
        [ProducesResponseType(typeof(TimeLogStatisticDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetStatistic()
            => (await _timeLogService.GetStatisticAsync()).ToActionResult();

        /// <summary>
        /// Create TimeLog.
        /// </summary>
        /// <param name="timeLogDto">workTime in minutes</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TimeLogResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> CreateTimeLog([FromBody] TimeLogCreateRequestDto timeLogDto)
            => (await _timeLogService.CreateAsync(timeLogDto)).ToActionResult();

        /// <summary>
        /// Update TimeLog.
        /// </summary>
        /// <param name="timeLogDto"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(TimeLogResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> UpdateTimeLog([FromBody] TimeLogUpdateRequestDto timeLogDto)
            => (await _timeLogService.UpdateAsync(timeLogDto)).ToActionResult();

        /// <summary>
        /// Delete TimeLog by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(TimeLogResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> DeleteTimeLog([Required] int id)
            => (await _timeLogService.DeleteAsync(id)).ToActionResult();
    }
}