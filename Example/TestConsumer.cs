using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
    public class TestConsumer : IConsumer<TestMessage>
    {
        private readonly ILogger _logger;
        public TestConsumer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("Consumer");
        }

        public async Task Consume(ConsumeContext<TestMessage> context)
        {
            var i = Counter.Increment();
            _logger.LogInformation($"Iteration `{i}`");
            await Task.Delay(5000);
            _logger.LogInformation($"Iteration `{i}` closed");
        }
    }
}
