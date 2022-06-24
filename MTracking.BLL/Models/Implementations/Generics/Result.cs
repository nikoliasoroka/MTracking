using System;
using System.Collections.Generic;
using MTracking.BLL.Models.Abstractions.Generics;

namespace MTracking.BLL.Models.Implementations.Generics
{
    internal class Result<TData> : IResult<TData>
    {
        public TData Data { get; private set; }

        public bool Success { get; private set; }

        public ErrorInfo ErrorInfo { get; private set; }

        private Result()
        { }

        public static Result<TData> CreateSuccess() => 
            new Result<TData> { Success = true };

        public static Result<TData> CreateFailed(string message, Exception exception = null) =>
            new Result<TData>
            {
                ErrorInfo = new ErrorInfo(message, exception)
            };

        public static Result<TData> CreateSuccess(TData data) =>
            new Result<TData>
            {
                Data = data,
                Success = true,
            };

        public virtual Result<TData> AddError(string message)
        {
            ErrorInfo.AddError(message);

            return this;
        }

        public virtual Result<TData> AddErrors(IEnumerable<string> collection)
        {
            ErrorInfo.AddErrors(collection);

            return this;
        }
    }
}
