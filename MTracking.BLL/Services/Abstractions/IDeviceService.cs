using System.Collections.Generic;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Device.Request;
using MTracking.BLL.Models.Abstractions;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IDeviceService
    {
        Task<IResult> AddOrUpdateDevice(DeviceDto deviceDto);

        Task<IResult> RemoveDevice(DeviceDto deviceDto);

        Task<IResult> ClearDevices(ICollection<string> failedTokens);
    }
}
