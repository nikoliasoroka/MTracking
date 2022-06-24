using Dasync.Collections;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Entities;
using MTracking.DAL.Repository;
using MTracking.DAL.UnitOfWork;

namespace MTracking.BLL.Services.Implementations
{
    public class PushService : IPushService
    {
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly IFirebaseService _firebaseService;

        public PushService(IUnitOfWork unitOfWork, IFirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
            _reminderRepository = unitOfWork.GetRepository<Reminder>();
        }

        public async Task<IResult> SendNotification()
        {
            var reminders = await GetTodaysRemindersAsync();

            if (!reminders.Any())
                return Result.CreateSuccess();

            await reminders.ParallelForEachAsync(item =>
            {
                var devices = item.User.Devices.Select(x => x.FirebaseToken).ToList();

                if (!devices.Any())
                    return Task.CompletedTask;

                return _firebaseService.SendNotification(devices, "Reminder", "Don't forget to track your time!");
            });

            return Result.CreateSuccess();
        }

        private async Task<List<Reminder>> GetTodaysRemindersAsync()
        {
            var now = DateTime.UtcNow;
            var reminders = _reminderRepository.Table
                .Include(x => x.User)
                .ThenInclude(x => x.Devices)
                .Where(x => x.IsActive && x.Hours == now.Hour && x.Minutes == now.Minute);

            return now.DayOfWeek switch
            {
                DayOfWeek.Monday => await reminders.Where(x => x.Monday).ToListAsync(),
                DayOfWeek.Tuesday => await reminders.Where(x => x.Tuesday).ToListAsync(),
                DayOfWeek.Wednesday => await reminders.Where(x => x.Wednesday).ToListAsync(),
                DayOfWeek.Thursday => await reminders.Where(x => x.Thursday).ToListAsync(),
                DayOfWeek.Friday => await reminders.Where(x => x.Friday).ToListAsync(),
                DayOfWeek.Saturday => await reminders.Where(x => x.Saturday).ToListAsync(),
                DayOfWeek.Sunday => await reminders.Where(x => x.Sunday).ToListAsync(),
                _ => null
            };
        }
    }
}
