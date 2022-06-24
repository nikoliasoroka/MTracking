using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Description.Request;
using MTracking.BLL.DTOs.Description.Response;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/description")]
    [ApiController]
    [Authorize]
    public class DescriptionController : ControllerBase
    {
        private readonly IDescriptionService _descriptionService;

        public DescriptionController(IDescriptionService descriptionService)
        {
            _descriptionService = descriptionService;
        }

        /// <summary>
        /// Get description by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DescriptionResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetDescription([Required] int id)
            => (await _descriptionService.GetByIdAsync(id)).ToActionResult();

        /// <summary>
        /// Get all descriptions.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<DescriptionResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
            => (await _descriptionService.GetAllAsync(paginationParams)).ToActionResult();

        /// <summary>
        /// Search descriptions by name.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<DescriptionResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Search([FromQuery] PaginationParams paginationParams, [FromQuery] string query = "")
            => (await _descriptionService.SearchDescriptionsAsync(paginationParams, query)).ToActionResult();

        /// <summary>
        /// Create custom description.
        /// </summary>
        /// <param name="descriptionDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(DescriptionResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> CreateCustomDescription([FromBody] DescriptionCreateRequestDto descriptionDto)
            => (await _descriptionService.CreateCustomAsync(descriptionDto)).ToActionResult();
        
        /// <summary>
        /// Update custom description.
        /// </summary>
        /// <param name="descriptionDto"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(DescriptionResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> UpdateDescription([FromBody] DescriptionUpdateRequestDto descriptionDto)
            => (await _descriptionService.UpdateAsync(descriptionDto)).ToActionResult();

        /// <summary>
        /// Delete custom description by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(DescriptionResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> DeleteDescription([Required] int id)
            => (await _descriptionService.DeleteAsync(id)).ToActionResult();
    }
}
