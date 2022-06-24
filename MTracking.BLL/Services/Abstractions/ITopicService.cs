using System;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Topic.Request;
using MTracking.BLL.DTOs.Topic.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;

namespace MTracking.BLL.Services.Abstractions
{
    public interface ITopicService : ICrudOperation<TopicResponseDto, TopicCreateRequestDto, TopicUpdateRequestDto>
    {
        Task<IResult<PagedResult<TopicResponseDto>>> GetAllAsync(PaginationParams paginationParams);
        Task<IResult<TopicResponseDto>> CreateCustomAsync(TopicCreateRequestDto dto);

        Task<IResult<PagedResult<TopicResponseDto>>> SearchTopicsAsync(PaginationParams paginationParams, string query);
    }
}