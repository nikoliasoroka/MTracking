using System.Threading.Tasks;
using MTracking.BLL.DTOs.Description.Request;
using MTracking.BLL.DTOs.Description.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IDescriptionService : ICrudOperation<DescriptionResponseDto, DescriptionCreateRequestDto, DescriptionUpdateRequestDto>
    {
        Task<IResult<PagedResult<DescriptionResponseDto>>> GetAllAsync(PaginationParams paginationParams);
        Task<IResult<DescriptionResponseDto>> CreateCustomAsync(DescriptionCreateRequestDto dto);
        Task<IResult<PagedResult<DescriptionResponseDto>>> SearchDescriptionsAsync(PaginationParams paginationParams, string query);
    }
}
