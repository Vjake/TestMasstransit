using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Monitoring.Health;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example
{
    public class Sender : IHostedService
    {
        readonly IBusControl _bus;
        readonly IBusHealth _busHealth;

        readonly ILogger _logger;

        public Sender(ILoggerFactory loggerFactory, IBusControl bus, IBusHealth busHealth)
        {
            _logger = loggerFactory.CreateLogger("Sender");
            _bus = bus;
            _busHealth = busHealth;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await WaitForHealthyBus(cancellationToken);
            for (var i = 0; i < 1000; i++)
            {
                try
                {
                    await _bus.Publish(new TestMessage
                    {
                        A1 = Guid.NewGuid(),
                        A2 = Guid.NewGuid(),
                        A3 = Guid.NewGuid(),
                        A4 = Guid.NewGuid(),
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Exception occured");
                    i--;
                }
            }
            _logger.LogInformation("Pushing has ended");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        async Task WaitForHealthyBus(CancellationToken cancellationToken)
        {
            HealthResult result;
            do
            {
                result = _busHealth.CheckHealth();

                await Task.Delay(100, cancellationToken);
            } while (result.Status != BusHealthStatus.Healthy);
        }
    }
}