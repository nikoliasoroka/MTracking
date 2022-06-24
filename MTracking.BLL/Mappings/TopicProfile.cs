using AutoMapper;
using MTracking.BLL.DTOs.Topic.Request;
using MTracking.BLL.DTOs.Topic.Response;
using MTracking.Core.Entities;

namespace MTracking.BLL.Mappings
{
    public class TopicProfile : Profile
    {
        public TopicProfile()
        {
            CreateMap<Topic, TopicResponseDto>().ReverseMap();

            CreateMap<Topic, TopicCreateRequestDto>().ReverseMap();

            CreateMap<TopicUpdateRequestDto, Topic>();
        }
    }
}
