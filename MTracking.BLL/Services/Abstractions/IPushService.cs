using System.Threading.Tasks;
using MTracking.BLL.Models.Abstractions;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IPushService
    {
        Task<IResult> SendNotification();
    }
}
