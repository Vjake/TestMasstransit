using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHost())
            {
                await host.RunAsync();
            }
        }

        public static Microsoft.Extensions.Hosting.IHost CreateHost() =>
            new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                ConfigureServices(services);
            })
            .UseConsoleLifetime()
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConsole();
            })
            .Build();

        private static void ConfigureServices(IServiceCollection collection)
        {
            collection.AddTransient<TestConsumer>();
            collection.AddMassTransit(cfg =>
            {
                cfg.AddBus(busRegistrationContext =>
                    {
                        var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            cfg.Host("localhost", 5672, "/", h =>
                            {
                                h.Username("admin");
                                h.Password("admin");
                            });

                            cfg.Message<TestMessage>(c => c.SetEntityName("test"));
                            EndpointConvention.Map<TestMessage>(
                                new Uri("rabbitmq://localhost/%2F/test"));

                            cfg.ReceiveEndpoint("test", configurator =>
                            {
                                configurator.PrefetchCount = 2;
                                configurator.Consumer(
                                    () => collection.BuildServiceProvider().GetRequiredService<TestConsumer>(),
                                    queueConfigurator =>
                                    {
                                        queueConfigurator.UseConcurrentMessageLimit(1);
                                    });
                            });
                        });
                        return bus;
                    }
                );
            });
            // before start create topology like test(exchange)->test(queue)
            // at rabbitmq
            collection.AddHostedService<Sender>();

            // when messages will be being consumed
            // kill rabbitmq node of cluster randomly and start over
            // after some iterations I get bug
            collection.AddHostedService<BusRunner>();
        }
    }
}
