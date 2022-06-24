using System.Collections.Generic;
using System.Threading.Tasks;
using MTracking.BLL.DTOs.Reminder.Request;
using MTracking.BLL.DTOs.Reminder.Response;
using MTracking.BLL.Models.Abstractions.Generics;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IReminderService : ICrudOperation<ReminderResponseDto, ReminderCreateRequestDto, ReminderUpdateRequestDto>
    {
        Task<IResult<IEnumerable<ReminderResponseDto>>> GetAllAsync();
    }
}
