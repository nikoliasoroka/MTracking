using System;
using System.Collections.Generic;

namespace MTracking.BLL.Models.Implementations
{
    public class ErrorInfo
    {
        private List<string> _messagesList = new List<string>();

        public string Error { get; }

        public IReadOnlyCollection<string> Messages => _messagesList;

        public Exception Exception { get; }

        public ErrorInfo()
        { }

        public ErrorInfo(string message, Exception exception)
        {
            Error = message;
            Exception = exception;
            _messagesList.Add(message);
        }

        public void AddError(string message)
        {
            _messagesList.Add(message);
        }

        public void AddErrors(IEnumerable<string> collection)
        {
            _messagesList.AddRange(collection);
        }
    }
}
