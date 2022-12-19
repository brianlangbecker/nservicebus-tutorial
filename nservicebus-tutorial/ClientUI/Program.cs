using System;
using Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Logging;

namespace ClientUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "ClientUI";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration("ClientUI");
                    var transport = endpointConfiguration.UseTransport<LearningTransport>();

                    var routing = transport.Routing();
                    routing.RouteToEndpoint(typeof(PlaceOrder), "Sales");

                    // Uncommeent if you want to use
                    // endpointConfiguration.SendFailedMessagesTo("error");
                    // endpointConfiguration.AuditProcessedMessagesTo("audit");
                    // endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");

                    // var metrics = endpointConfiguration.EnableMetrics();
                    // metrics.SendMetricDataToServiceControl(
                    //     "Particular.Monitoring",
                    //     TimeSpan.FromMilliseconds(500)
                    // );

                    // Required to turn on OTel in NserviceBus.Core
                    endpointConfiguration.EnableOpenTelemetry();
                    return endpointConfiguration;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
