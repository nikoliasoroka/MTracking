using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Reminder.Request;
using MTracking.BLL.DTOs.Reminder.Response;
using MTracking.BLL.Models.Abstractions.Generics;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Entities;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class ReminderService : IReminderService
    {
        private readonly IUserInfoService _userInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Reminder> _reminderRepository;

        public ReminderService(IUnitOfWork unitOfWork, IMapper mapper, IUserInfoService userInfo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userInfo = userInfo;
            _userRepository = unitOfWork.GetRepository<User>();
            _reminderRepository = unitOfWork.GetRepository<Reminder>();
        }

        public async Task<IResult<ReminderResponseDto>> GetByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.Table
                    .Include(x => x.Reminders)
                    .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

                if (user is null)
                    return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

                var reminder = user.Reminders
                    .FirstOrDefault(x => x.Id == id);

                if (reminder is null)
                    return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotFound);

                var result = _mapper.Map<ReminderResponseDto>(reminder);

                if (_userInfo.UserTime is null)
                    return Result<ReminderResponseDto>.CreateSuccess(_mapper.Map<ReminderResponseDto>(result));

                var reminderHours = new DateTimeOffset(_userInfo.UserTime.Value.Year, _userInfo.UserTime.Value.Month, _userInfo.UserTime.Value.Day,
                                                    result.Hours, 0, 0, _userInfo.UserTime.Value.Offset);

                var reminderClientsTime = reminderHours.AddHours(_userInfo.UserTime.Value.Offset.Hours);
                result.Hours = reminderClientsTime.Hour;

                return Result<ReminderResponseDto>.CreateSuccess(_mapper.Map<ReminderResponseDto>(result));
            }
            catch (Exception exception)
            {
                return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotFound).AddError(exception.Message);
            }
        }

        public async Task<IResult<IEnumerable<ReminderResponseDto>>> GetAllAsync()
        {
            try
            {
                var reminders = await _reminderRepository.Table
                    .Where(x => x.UserId == _userInfo.UserId)
                    .ToListAsync();

                var reminderList = _mapper.Map<IEnumerable<ReminderResponseDto>>(reminders).ToList();

                if (!reminderList.Any())
                    return Result<IEnumerable<ReminderResponseDto>>.CreateSuccess(reminderList);

                if (_userInfo.UserTime is null)
                    return Result<IEnumerable<ReminderResponseDto>>.CreateSuccess(reminderList);

                foreach (var reminder in reminderList)
                {
                    var reminderHours = new DateTimeOffset(_userInfo.UserTime.Value.Year, _userInfo.UserTime.Value.Month, _userInfo.UserTime.Value.Day,
                        reminder.Hours, 0, 0, _userInfo.UserTime.Value.Offset);

                    var reminderClientsTime = reminderHours.AddHours(_userInfo.UserTime.Value.Offset.Hours);
                    reminder.Hours = reminderClientsTime.Hour;
                }

                return Result<IEnumerable<ReminderResponseDto>>.CreateSuccess(reminderList);
            }
            catch (Exception exception)
            {
                return Result<IEnumerable<ReminderResponseDto>>.CreateFailed(ValidationFactory.ReminderIsNotFound).AddError(exception.Message);
            }
        }

        public async Task<IResult<ReminderResponseDto>> CreateAsync(ReminderCreateRequestDto dto)
        {
            try
            {
                var reminders = await _reminderRepository.Table
                    .Where(x => x.UserId == _userInfo.UserId)
                    .ToListAsync();

                if (reminders.Count >= 5)
                    return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.CannotCreateMoreThan5Reminders);

                var newReminder = _mapper.Map<Reminder>(dto);

                if (!IsDaySelected(newReminder))
                    newReminder.GetType().GetProperty(DateTime.UtcNow.DayOfWeek.ToString())?.SetValue(newReminder, true);

                newReminder.UserId = _userInfo.UserId;

                if (_userInfo.UserTime != null)
                {
                    var reminderHours = new DateTimeOffset(_userInfo.UserTime.Value.Year, _userInfo.UserTime.Value.Month, _userInfo.UserTime.Value.Day,
                        newReminder.Hours, 0, 0, _userInfo.UserTime.Value.Offset);

                    var reminderClientsTime = reminderHours.AddHours(-_userInfo.UserTime.Value.Offset.Hours);
                    newReminder.Hours = reminderClientsTime.Hour;
                }

                var result = await _reminderRepository.InsertAsync(newReminder);

                if (await _unitOfWork.SaveAsync() <= 0)
                    return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotCreated);

                result.Hours = dto.Hours;

                return Result<ReminderResponseDto>.CreateSuccess(_mapper.Map<ReminderResponseDto>(result));
            }
            catch (Exception exception)
            {
                return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotCreated).AddError(exception.Message);
            }
        }

        public async Task<IResult<ReminderResponseDto>> UpdateAsync(ReminderUpdateRequestDto dto)
        {
            try
            {
                var user = await _userRepository.Table
                    .Include(x => x.Reminders)
                    .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

                if (user is null)
                    return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

                var reminder = user.Reminders
                    .FirstOrDefault(x => x.Id == dto.Id);

                if (reminder is null)
                    return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotUpdated).AddError(ValidationFactory.ReminderIsNotFound);

                var reminderToUpdate = _mapper.Map(dto, reminder);

                if (_userInfo.UserTime != null)
                {
                    var reminderHours = new DateTimeOffset(_userInfo.UserTime.Value.Year, _userInfo.UserTime.Value.Month, _userInfo.UserTime.Value.Day,
                        reminder.Hours, 0, 0, _userInfo.UserTime.Value.Offset);

                    var reminderClientsTime = reminderHours.AddHours(-_userInfo.UserTime.Value.Offset.Hours);
                    reminderToUpdate.Hours = reminderClientsTime.Hour;
                }

                var updatedReminder = await _reminderRepository.UpdateAsync(reminderToUpdate);

                if (await _unitOfWork.SaveAsync() <= 0)
                    return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotCreated);

                updatedReminder.Hours = dto.Hours;

                return Result<ReminderResponseDto>.CreateSuccess(_mapper.Map<ReminderResponseDto>(updatedReminder));
            }
            catch (Exception exception)
            {
                return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotUpdated).AddError(exception.Message);
            }
        }

        public async Task<IResult<ReminderResponseDto>> DeleteAsync(int id)
        {
            var user = await _userRepository.Table
                .Include(x => x.Reminders)
                .FirstOrDefaultAsync(x => x.Id == _userInfo.UserId);

            if (user is null)
                return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.AccessDenied);

            var reminder = user.Reminders
                .FirstOrDefault(x => x.Id == id);

            if (reminder is null)
                return Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotDeleted).AddError(ValidationFactory.ReminderIsNotFound);

            await _reminderRepository.DeleteAsync(reminder.Id);

            return await _unitOfWork.SaveAsync() > 0
                ? Result<ReminderResponseDto>.CreateSuccess(_mapper.Map<ReminderResponseDto>(reminder))
                : Result<ReminderResponseDto>.CreateFailed(ValidationFactory.ReminderIsNotDeleted);
        }

        private bool IsDaySelected(Reminder reminder)
        {
            var daysSelected = new List<bool>();

            foreach (var day in CultureInfo.InvariantCulture.DateTimeFormat.DayNames)
            {
                daysSelected.Add((bool)reminder.GetType().GetProperty(day).GetValue(reminder, null));
            }

            return daysSelected.Contains(true);
        }
    }
}
