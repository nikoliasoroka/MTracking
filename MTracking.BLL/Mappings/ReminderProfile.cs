using AutoMapper;
using MTracking.BLL.DTOs.Reminder.Request;
using MTracking.BLL.DTOs.Reminder.Response;
using MTracking.Core.Entities;

namespace MTracking.BLL.Mappings
{
    public class ReminderProfile : Profile
    {
        public ReminderProfile()
        {
            CreateMap<Reminder, ReminderResponseDto>().ReverseMap();

            CreateMap<Reminder, ReminderCreateRequestDto>().ReverseMap();

            CreateMap<ReminderUpdateRequestDto, Reminder>();
        }
    }
}
