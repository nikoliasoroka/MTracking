using System;

namespace MTracking.BLL.Services.Abstractions
{
    public interface IUserInfoService
    {
        public string UserName { get; }
        public int UserId { get; }
        public bool IsAuthenticated { get; }
        public DateTimeOffset? UserTime { get; set; }
    }
}
