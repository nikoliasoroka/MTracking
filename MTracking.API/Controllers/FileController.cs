using System;
using MTracking.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper.Mappers;
using MTracking.BLL.DTOs.File.Request;
using MTracking.BLL.DTOs.File.Response;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.API.Controllers
{
    [Route("api/file")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Get file by Id.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpGet("{fileId:int}")]
        [ProducesResponseType(typeof(FileResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetFileById([Required] int fileId)
            => (await _fileService.GetByIdAsync(fileId)).ToActionResult();

        /// <summary>
        /// Get files by user.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("by-user")]
        [ProducesResponseType(typeof(PagedResult<FileResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetAllFilesByUserId([FromQuery] PaginationParams paginationParams)
            => (await _fileService.GetFilesByUserAsync(paginationParams)).ToActionResult();

        /// <summary>
        /// Get all files.
        /// </summary>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedResult<FileResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> GetAllFiles([FromQuery] PaginationParams paginationParams)
            => (await _fileService.GetAllAsync(paginationParams)).ToActionResult();

        /// <summary>
        /// Search files by name.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="paginationParams"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedResult<FileResponseDto>), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Search([FromQuery] PaginationParams paginationParams,
            [FromQuery] string query = "")
            => (await _fileService.SearchFilesAsync(paginationParams, query)).ToActionResult();

        /// <summary>
        /// Create file by user.
        /// </summary>
        /// <param name="fileDto"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [ProducesResponseType(typeof(FileResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> CreateFileByUser([FromBody] FileCreateRequestDto fileDto)
            => (await _fileService.CreateAsync(fileDto)).ToActionResult();

        /// <summary>
        /// Create common file.
        /// </summary>
        /// <param name="fileDto"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("common")]
        [ProducesResponseType(typeof(FileResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> CreateFile([FromBody] FileCreateRequestDto fileDto)
            => (await _fileService.CreateCommonFileAsync(fileDto)).ToActionResult();

        /// <summary>
        /// Update file.
        /// </summary>
        /// <param name="fileDto"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [ProducesResponseType(typeof(FileResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> UpdateFile([FromBody] FileUpdateRequestDto fileDto)
            => (await _fileService.UpdateAsync(fileDto)).ToActionResult();

        /// <summary>
        /// Delete file.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("{fileId:int}")]
        [ProducesResponseType(typeof(FileResponseDto), 200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> DeleteFile([Required] int fileId)
            => (await _fileService.DeleteAsync(fileId)).ToActionResult();

        /// <summary>
        /// Pin file.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpPut("Pin/{fileId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> Pin([Required] int fileId)
            => (await _fileService.PinFileAsync(true, fileId)).ToActionResult();

        /// <summary>
        /// UnPin file.
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpPut("UnPin/{fileId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorInfo), 400)]
        public async Task<IActionResult> UnPin([Required] int fileId)
            => (await _fileService.PinFileAsync(false, fileId)).ToActionResult();
    }
}