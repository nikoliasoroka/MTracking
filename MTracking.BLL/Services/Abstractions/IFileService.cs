using System;
using System.Threading.Tasks;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.DTOs.File.Request;
using MTracking.BLL.DTOs.File.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IFileService : ICrudOperation<FileResponseDto, FileCreateRequestDto, FileUpdateRequestDto>
    {
        Task<IResult<PagedResult<FileResponseDto>>> GetFilesByUserAsync(PaginationParams paginationParams);
        Task<IResult<PagedResult<FileResponseDto>>> GetAllAsync(PaginationParams paginationParams);
        Task<IResult<FileResponseDto>> CreateCommonFileAsync(FileCreateRequestDto fileDt);

        Task<IResult<PagedResult<FileResponseDto>>> SearchFilesAsync(PaginationParams paginationParams, string query);

        Task<IResult<FileResponseDto>> PinFileAsync(bool status, int fileId);
    }
}