using AutoMapper;
using MTracking.BLL.DTOs.Description.Request;
using MTracking.BLL.DTOs.Description.Response;
using MTracking.Core.Entities;

namespace MTracking.BLL.Mappings
{
    public class DescriptionProfile : Profile
    {
        public DescriptionProfile()
        {
            CreateMap<Description, DescriptionResponseDto>().ReverseMap();

            CreateMap<Description, DescriptionCreateRequestDto>().ReverseMap();

            CreateMap<DescriptionUpdateRequestDto, Description>();
        }
    }
}
