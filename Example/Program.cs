using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IHost = Microsoft.Extensions.Hosting.IHost;

namespace Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHost();

            await host.RunAsync();
        }

        public static IHost CreateHost()
        {
            return new HostBuilder()
                .ConfigureServices((hostContext, services) => { ConfigureServices(services); })
                .UseConsoleLifetime()
                .ConfigureLogging((hostingContext, logging) => { logging.AddConsole(); })
                .Build();
        }

        static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<TestConsumer>(c => c.UseConcurrentMessageLimit(1));

                x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", 5672, "/", h =>
                        {
                            h.Username("admin");
                            h.Password("admin");
                        });

                        cfg.Message<TestMessage>(c => c.SetEntityName("test"));
                        EndpointConvention.Map<TestMessage>(new Uri("rabbitmq://localhost/%2F/test"));

                        cfg.ReceiveEndpoint("test", configurator =>
                        {
                            configurator.PrefetchCount = 1;
                            configurator.ConfigureConsumer<TestConsumer>(context);
                        });
                    }
                );
            });

            // when messages will be being consumed
            // kill rabbitmq node of cluster randomly and start over
            // after some iterations I get bug
            collection.AddHostedService<BusRunner>();

            // before start create topology like test(exchange)->test(queue)
            // at rabbitmq
            collection.AddHostedService<Sender>();
        }
    }
}