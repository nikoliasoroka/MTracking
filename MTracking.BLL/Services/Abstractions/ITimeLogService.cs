using System;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.TimeLog.Request;
using MTracking.BLL.DTOs.TimeLog.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;

namespace MTracking.BLL.Services.Abstractions
{
    public interface
        ITimeLogService : ICrudOperation<TimeLogResponseDto, TimeLogCreateRequestDto, TimeLogUpdateRequestDto>
    {
        Task<IResult<PagedResult<TimeLogResponseDto>>> GetAllAsync(PaginationParams paginationParams);
        Task<IResult<PagedResult<TimeLogResponseDto>>> GetDailyAsync(PaginationParams paginationParams);
        Task<IResult<PagedResult<TimeLogResponseDto>>> GetMonthlyAsync(PaginationParams paginationParams);

        Task<IResult<PagedResult<TimeLogResponseDto>>> SearchAsync(PaginationParams paginationParams, string query,
            DateTime? DateFrom, DateTime? DateTo);

        Task<IResult<PagedResult<TimeLogResponseDto>>> SearchMonthlyAsync(PaginationParams paginationParams,
            string query);

        Task<IResult<TimeLogStatisticDto>> GetStatisticAsync();
    }
}