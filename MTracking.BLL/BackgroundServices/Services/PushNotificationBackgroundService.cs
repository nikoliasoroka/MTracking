using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MTracking.BLL.Services.Abstractions;

namespace MTracking.BLL.BackgroundServices.Services
{
    public class PushNotificationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PushNotificationBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var pushService = scope.ServiceProvider.GetRequiredService<IPushService>();

            await Task.Delay(30000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await pushService.SendNotification();

                Debug.WriteLine($"Run {nameof(PushNotificationBackgroundService)}");

                await Task.Delay(55000, stoppingToken);
            }
        }
    }
}