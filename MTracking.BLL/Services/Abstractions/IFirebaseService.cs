using System.Collections.Generic;
using System.Threading.Tasks;
using MTracking.BLL.Models.Abstractions;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IFirebaseService
    {
        Task<IResult> SendNotification(List<string> clientTokens, string title, string body, Dictionary<string, string> data = null);
    }
}
