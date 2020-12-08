using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example
{
    public class Sender : IHostedService
    {
        private readonly IBusControl _bus;

        private readonly ILogger _logger;
        public Sender(
            ILoggerFactory loggerFactory,
            IBusControl bus)
        {
            _logger = loggerFactory.CreateLogger("Sender");
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start pushing");
            for (int i = 1; i < 30000; i++)
            {
                await _bus.Publish(new TestMessage
                {
                    A1 = Guid.NewGuid(),
                    A2 = Guid.NewGuid(),
                    A3 = Guid.NewGuid(),
                    A4 = Guid.NewGuid(),
                });
            };
            _logger.LogInformation("Start pushing");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
