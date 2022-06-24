using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Logger;

namespace MTracking.BLL.Services.Implementations
{
    public class UserInfo : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public string UserName { get; set; }
        public int UserId { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTimeOffset? UserTime { get; set; }

        public UserInfo(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    UserTime = GetDateTimeOffset();
                    IsAuthenticated = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
                    if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity)
                    {
                        UserName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                        UserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                    }
                }
            }
            catch (Exception exception)
            {
                IsAuthenticated = false;
                UserId = -1;
                UserName = "Not Authenticated";

                Logger.Error("UserInfo service", exception);
            }
        }

        private DateTimeOffset? GetDateTimeOffset()
        {
            var dateHeader = _httpContextAccessor.HttpContext.Request.Headers["Local-DateTime"];

            return DateTimeOffset.TryParseExact(dateHeader, "dd.MM.yyyy HH:mm:ss zzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) 
                ? result 
                : (DateTimeOffset?)null;
        }
    }
}
