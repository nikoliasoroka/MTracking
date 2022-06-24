using System.Threading.Tasks;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Implementations;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IEmailService
    {
        Task<IResult> SendEmailAsync(EmailMessage message);
    }
}
