using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MTracking.BLL.Models.Abstractions;
using MTracking.BLL.Models.Implementations;
using MTracking.BLL.Models.Implementations.Generics;
using MTracking.BLL.Services.Abstractions;
using MTracking.Core.Constants;
using MTracking.Core.Logger;

namespace MTracking.BLL.Services.Implementations
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IDeviceService _deviceService;

        public FirebaseService(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        public async Task<IResult> SendNotification(List<string> clientTokens, string title, string body, Dictionary<string, string> data = null)
        {
            try
            {
                var registrationTokens = clientTokens;
                var message = new MulticastMessage
                {
                    Tokens = registrationTokens,
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Badge = 1,
                            ContentAvailable = true,
                            CriticalSound = new CriticalSound()
                            {
                                Critical = true,
                                Name = "MTracking sound"
                            }
                        },
                        Headers = new Dictionary<string, string>()
                        {
                            { "apns-priority", "10" }
                        },
                    },
                    Notification = new Notification
                    {
                        Body = body,
                        Title = title
                    },
                    Data = data
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).ConfigureAwait(true);
                Logger.Info($"Push Success: {string.Join(", ", response.Responses.Where(x => x.IsSuccess).Select(x => x.MessageId).ToList())}");

                if (response.FailureCount > 0)
                {
                    Logger.Error($"Push Error: {string.Join(", ", response.Responses.Where(x => !x.IsSuccess).Select(x => x.Exception.Message).ToList())}");

                    var result = Result.CreateFailed(ValidationFactory.FirebaseError);
                    var failedTokens = new List<string>();

                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                        {
                            switch (response.Responses[i].Exception.MessagingErrorCode)
                            {
                                case MessagingErrorCode.Unregistered:
                                case MessagingErrorCode.Unavailable:
                                    failedTokens.Add(registrationTokens[i]);
                                    break;
                            }
                            result.AddError($"Token: {registrationTokens[i]}; Exception: {response.Responses[i].Exception.Message}");
                        }
                        else
                        {
                            result.AddError($"Token: {registrationTokens[i]} - SUCCESS");
                        }
                    }

                    await _deviceService.ClearDevices(failedTokens);

                    return result;
                }

                return Result<List<string>>.CreateSuccess(response.Responses.Select(x => x.MessageId).ToList());
            }
            catch (Exception exception)
            {
                return Result.CreateFailed(exception.Message);
            }
        }
    }
}
