using AutoMapper;
using MTracking.BLL.DTOs.TimeLog.Request;
using MTracking.BLL.DTOs.TimeLog.Response;
using MTracking.Core.Entities;
using MTracking.Core.Enums;

namespace MTracking.BLL.Mappings
{
    public class TimeLogProfile : Profile
    {
        public TimeLogProfile()
        {
            CreateMap<TimeLog, TimeLogResponseDto>()
                .ForMember(x => x.Date, opt => opt.MapFrom(src => src.Date.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")))
                .ForMember(x => x.IsEditable,
                    opt => opt.MapFrom(src => src.BillingStatus != CommitBillingStatus.Charged))
                .ForMember(x => x.CommitRecordId, opt => opt.MapFrom(src => src.CommitRecordId));

            CreateMap<TimeLogCreateRequestDto, TimeLog>()
                .ForMember(dest => dest.TopicId, act => act.MapFrom(src => src.TopicId == 0 ? null : src.TopicId))
                .ForMember(dest => dest.DescriptionId,
                    act => act.MapFrom(src => src.DescriptionId == 0 ? null : src.DescriptionId))
                .ForMember(x => x.ForExport, opt => opt.MapFrom(o => true))
                .ReverseMap();

            CreateMap<TimeLogUpdateRequestDto, TimeLog>()
                .ForMember(dest => dest.TopicId, act => act.MapFrom(src => src.TopicId == 0 ? null : src.TopicId))
                .ForMember(dest => dest.DescriptionId,
                    act => act.MapFrom(src => src.DescriptionId == 0 ? null : src.DescriptionId))
                .ForMember(x => x.ForExport, opt => opt.MapFrom(o => true));
        }
    }
}