using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace Billing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Billing";
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        static class CustomActivitySources
        {
            public const string Name = "Sample.ActivitySource";
            public static ActivitySource Main = new ActivitySource(Name);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var serviceName = "BillingProcess";
            var serviceVersion = "1.0.0";

            return Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration("Billing");

                    endpointConfiguration.UseTransport<LearningTransport>();

                    endpointConfiguration.SendFailedMessagesTo("error");
                    endpointConfiguration.AuditProcessedMessagesTo("audit");
                    endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");

                    var metrics = endpointConfiguration.EnableMetrics();
                    metrics.SendMetricDataToServiceControl(
                        "Particular.Monitoring",
                        TimeSpan.FromMilliseconds(500)
                    );

                    endpointConfiguration.EnableOpenTelemetry();

                    var tracerProvider = Sdk.CreateTracerProviderBuilder()
                        .AddOtlpExporter(option =>
                        {
                            option.Endpoint = new Uri("https://api.honeycomb.io/v1/traces");
                            option.Headers = "x-honeycomb-team=X2ojNtHwCSbLqoT6cudreH";
                            option.Protocol = OtlpExportProtocol.HttpProtobuf;
                        })
                        .SetResourceBuilder(
                            ResourceBuilder
                                .CreateDefault()
                                .AddService(
                                    serviceName: serviceName,
                                    serviceVersion: serviceVersion
                                )
                        )
                        .AddSource("NServiceBus.core")
                        .AddSource(CustomActivitySources.Name)
                        .Build();

                    return endpointConfiguration;
                });
        }
    }
}
