using System;
using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Topic.Request;
using MTracking.BLL.DTOs.Topic.Response;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/topic")]
    [ApiController]
    [Authorize]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        /// <summary>
        /// Get topic by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TopicResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetTopic([Required] int id)
            => (await _topicService.GetByIdAsync(id)).ToActionResult();

        /// <summary>
        /// Get all topics.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<TopicResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
            => (await _topicService.GetAllAsync(paginationParams)).ToActionResult();

        /// <summary>
        /// Search topics by name.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<TopicResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Search([FromQuery] PaginationParams paginationParams,
            [FromQuery] string query = "")
            => (await _topicService.SearchTopicsAsync(paginationParams, query)).ToActionResult();

        /// <summary>
        /// Create custom topic.
        /// </summary>
        /// <param name="topicDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TopicResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> CreateCustomTopic([FromBody] TopicCreateRequestDto topicDto)
            => (await _topicService.CreateCustomAsync(topicDto)).ToActionResult();

        /// <summary>
        /// Update custom topic.
        /// </summary>
        /// <param name="TopicDto"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [ProducesResponseType(typeof(TopicResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> UpdateTopic([FromBody] TopicUpdateRequestDto TopicDto)
            => (await _topicService.UpdateAsync(TopicDto)).ToActionResult();

        /// <summary>
        /// Delete custom topic by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(TopicResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> DeleteTopic([Required] int id)
            => (await _topicService.DeleteAsync(id)).ToActionResult();
    }
}