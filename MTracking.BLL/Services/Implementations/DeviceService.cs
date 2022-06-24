using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MTracking.BLL.DTOs.Device.Request;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class DeviceService : IDeviceService
    {
        private readonly IUserInfoService _userInfo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Device> _deviceRepository;

        public DeviceService(IMapper mapper, IUnitOfWork unitOfWork, IUserInfoService userInfo)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userInfo = userInfo;
            _deviceRepository = unitOfWork.GetRepository<Device>();
        }

        public async Task<IResult> AddOrUpdateDevice(DeviceDto deviceDto)
        {
            var device = await _deviceRepository.Table
                .FirstOrDefaultAsync(x => x.ApplicationType == deviceDto.ApplicationType && x.FirebaseToken == deviceDto.FirebaseToken);

            if (device is null)
            {
                device = _mapper.Map<Device>(deviceDto);
                device.UserId = _userInfo.UserId;

                await _deviceRepository.InsertAsync(device);
            }
            else
            {
                device.UserId = _userInfo.UserId;
                device.FirebaseToken = deviceDto.FirebaseToken;

                await _deviceRepository.UpdateAsync(device);
            }
            
            return await _unitOfWork.SaveAsync() > 0
                ? Result.CreateSuccess()
                : Result.CreateFailed(ValidationFactory.DeviceIsNotRegistered);
        }

        public async Task<IResult> ClearDevices(ICollection<string> failedTokens)
        {
            try
            {
                var devicesIdToClear = await _deviceRepository.Table
                    .Where(x => failedTokens.Contains(x.FirebaseToken))
                    .Select(x => x.Id)
                    .ToListAsync();

                foreach (var deviceId in devicesIdToClear)
                {
                    await _deviceRepository.DeleteAsync(deviceId);
                }

                await _unitOfWork.SaveAsync();

                return Result.CreateSuccess();
            }
            catch
            {
                return Result.CreateFailed(ValidationFactory.DeviceIsNotUpdated);
            }
        }

        public async Task<IResult> RemoveDevice(DeviceDto removeDeviceDto)
        {
            var device = await _deviceRepository.Table
                .FirstOrDefaultAsync(x => x.ApplicationType == removeDeviceDto.ApplicationType && x.FirebaseToken == removeDeviceDto.FirebaseToken);

            if (device is null)
            {
                return Result.CreateFailed(ValidationFactory.DeviceIsNotRegistered);
            }
            else
            {
                await _deviceRepository.DeleteAsync(device.Id);
            }

            return await _unitOfWork.SaveAsync() > 0
                ? Result.CreateSuccess()
                : Result.CreateFailed(ValidationFactory.DeviceIsNotRegistered);
        }
    }
}
