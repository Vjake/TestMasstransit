using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example
{
    public class BusRunner : IHostedService
    {
        private readonly IBusControl _bus;

        public BusRunner(IBusControl bus)
        {
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync();
        }
    }
}
