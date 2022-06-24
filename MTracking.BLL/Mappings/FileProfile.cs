using System.Linq;
using AutoMapper;
using MTracking.BLL.DTOs.File.Request;
using MTracking.BLL.DTOs.File.Response;
using MTracking.Core.Entities;
using MTracking.Core.Enums;

namespace MTracking.BLL.Mappings
{
    public class FileProfile : Profile
    {
        public FileProfile()
        {
            CreateMap<File, FileResponseDto>()
                .ForMember(x => x.IsActive, opt => opt.MapFrom(src => src.PortfolioStatus == CommitFileStatus.Active))
                .ForMember(x => x.IsBillable, opt => opt.MapFrom(src => src.IsChargedCase))
                .ForMember(x => x.IsPinned, opt => opt.MapFrom(src => src.UserFilePins.Any()))
                .ReverseMap();

            CreateMap<File, FileCreateRequestDto>().ReverseMap();

            CreateMap<FileUpdateRequestDto, File>();
        }
    }
}