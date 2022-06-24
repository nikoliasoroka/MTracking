using System;
using System.Collections.Generic;
using MTracking.BLL.Models.Abstractions;

namespace MTracking.BLL.Models.Implementations
{
    internal class Result : IResult
    {
        public bool Success { get; private set; }

        public ErrorInfo ErrorInfo { get; private set; }

        private Result()
        { }

        public static Result CreateSuccess() => 
            new Result { Success = true };

        public static Result CreateFailed(string message, Exception exception = null) => 
            new Result
            {
                ErrorInfo = new ErrorInfo(message, exception)
            };

        public virtual Result AddError(string message)
        {
            ErrorInfo.AddError(message);

            return this;
        }

        public virtual Result AddErrors(IEnumerable<string> collection)
        {
            ErrorInfo.AddErrors(collection);

            return this;
        }
    }
}
