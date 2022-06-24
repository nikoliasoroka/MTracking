using AutoMapper;
using MTracking.BLL.DTOs.Device.Request;
using MTracking.Core.Entities;

namespace MTracking.BLL.Mappings
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<DeviceDto, Device>().ReverseMap();
        }
    }
}
