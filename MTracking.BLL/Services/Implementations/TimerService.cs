using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Timer.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class TimerService : ITimerService
    {
        private readonly IUserInfoService _userInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<Timer> _timerRepository;

        public TimerService(IUnitOfWork unitOfWork, IMapper mapper, IUserInfoService userInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userInfo = userInfo;
            _timerRepository = unitOfWork.GetRepository<Timer>();
        }

        public async Task<IResult<TimerResponseDto>> GetAsync()
        {
            var timer = await _timerRepository.Table
                .FirstOrDefaultAsync(x => x.UserId == _userInfo.UserId);

            if (timer is null)
                return Result<TimerResponseDto>.CreateSuccess(new TimerResponseDto { Duration = 0, IsTimerActive = false });

            if (!timer.IsTimerActive)
                return Result<TimerResponseDto>.CreateSuccess(_mapper.Map<TimerResponseDto>(timer));

            var duration = (DateTime.UtcNow - timer.StartTime).Value.TotalSeconds;

            timer.Duration += Convert.ToInt64(duration);

            if (timer.Duration >= 86100)
            {
                await _timerRepository.DeleteAsync(timer.Id);
                await _unitOfWork.SaveAsync();

                return Result<TimerResponseDto>.CreateSuccess(new TimerResponseDto { Duration = 0, IsTimerActive = false });
            }

            return Result<TimerResponseDto>.CreateSuccess(_mapper.Map<TimerResponseDto>(timer));
        }

        public async Task<IResult<TimerResponseDto>> StartAsync()
        {
            try
            {
                var timer = await _timerRepository.Table
                    .FirstOrDefaultAsync(x => x.UserId == _userInfo.UserId);

                if (timer != null)
                    return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerAlreadyExists);

                var newTimer = await _timerRepository.InsertAsync(new Timer
                {
                    Duration = 0,
                    IsTimerActive = true,
                    StartTime = DateTime.UtcNow,
                    UserId = _userInfo.UserId
                });

                return await _unitOfWork.SaveAsync() > 0
                    ? Result<TimerResponseDto>.CreateSuccess(_mapper.Map<TimerResponseDto>(newTimer))
                    : Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotCreated);
            }
            catch (Exception e)
            {
                return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotCreated, e);
            }
        }

        public async Task<IResult<TimerResponseDto>> StopAsync()
        {
            try
            {
                var timer = await _timerRepository.Table
                    .FirstOrDefaultAsync(x => x.UserId == _userInfo.UserId);

                if (timer is null)
                    return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotFound);

                if (!timer.IsTimerActive)
                    return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsStopped);

                var duration = (DateTime.UtcNow - timer.StartTime).Value.TotalSeconds;

                timer.Duration += Convert.ToInt64(duration);
                timer.StopTime = DateTime.UtcNow;
                timer.IsTimerActive = false;

                var result = await _timerRepository.UpdateAsync(timer);

                return await _unitOfWork.SaveAsync() > 0
                    ? Result<TimerResponseDto>.CreateSuccess(_mapper.Map<TimerResponseDto>(result))
                    : Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotStopped);
            }
            catch (Exception e)
            {
                return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotStopped, e);
            }
        }

        public async Task<IResult<TimerResponseDto>> ContinueAsync()
        {
            var timer = await _timerRepository.Table
                .FirstOrDefaultAsync(x => x.UserId == _userInfo.UserId);

            if (timer is null)
                return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotFound).AddError(ValidationFactory.TimerIsNotStarted);

            if (timer.IsTimerActive)
                return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotStopped);

            timer.StartTime = DateTime.UtcNow;
            timer.IsTimerActive = true;

            var result = await _timerRepository.UpdateAsync(timer);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TimerResponseDto>.CreateSuccess(_mapper.Map<TimerResponseDto>(result))
                : Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotStarted);
        }

        public async Task<IResult<TimerResponseDto>> ResetAsync()
        {
            var timer = await _timerRepository.Table
                .FirstOrDefaultAsync(x => x.UserId == _userInfo.UserId);

            if (timer is null)
                return Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotStarted).AddError(ValidationFactory.TimerIsNotReset);

            await _timerRepository.DeleteAsync(timer.Id);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<TimerResponseDto>.CreateSuccess(_mapper.Map<TimerResponseDto>(timer))
                : Result<TimerResponseDto>.CreateFailed(ValidationFactory.TimerIsNotReset);
        }
    }
}
