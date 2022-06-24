using MTracking.BLL.Models.Implementations;

namespace MTracking.BLL.Models.Abstractions
{
    public interface IResult
    {
        bool Success { get; }

        ErrorInfo ErrorInfo { get; }
    }
}
