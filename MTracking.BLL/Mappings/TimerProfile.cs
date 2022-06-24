using AutoMapper;
using MTracking.BLL.DTOs.Timer.Response;
using MTracking.Core.Entities;

namespace MTracking.BLL.Mappings
{
    public class TimerProfile : Profile
    {
        public TimerProfile()
        {
            CreateMap<Timer, TimerResponseDto>().ReverseMap();
        }
    }
}
