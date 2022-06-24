using System.Threading.Tasks;
using MTracking.BLL.DTOs.Timer.Response;
using MTracking.BLL.Models.Abstractions.Generics;

namespace MTracking.BLL.Services.Abstractions
{
    public interface ITimerService
    {
        Task<IResult<TimerResponseDto>> GetAsync();
        Task<IResult<TimerResponseDto>> StartAsync();
        Task<IResult<TimerResponseDto>> StopAsync();
        Task<IResult<TimerResponseDto>> ContinueAsync();
        Task<IResult<TimerResponseDto>> ResetAsync();
    }
}
