using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Sales
{
    public class PlaceOrderHandler : IHandleMessages<PlaceOrder>
    {
        static readonly ILog log = LogManager.GetLogger<PlaceOrderHandler>();

        static readonly Random random = new Random();

        static class CustomActivitySources
        {
            public const string Name = "Sample.ActivitySource";
            public static ActivitySource Main = new ActivitySource(Name);
        }

        public Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            var serviceName = "SalesProcess";
            var serviceVersion = "1.0.0";

            // var tracerProvider = Sdk.CreateTracerProviderBuilder()
            //     .AddOtlpExporter(option =>
            //     {
            //         option.Endpoint = new Uri("https://api.honeycomb.io/v1/traces");
            //         option.Headers = "x-honeycomb-team=X2ojNtHwCSbLqoT6cudreH";
            //         option.Protocol = OtlpExportProtocol.HttpProtobuf;
            //     })
            //     .SetResourceBuilder(
            //         ResourceBuilder
            //             .CreateDefault()
            //             .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
            //     )
            //     .AddSource("NServiceBus.core")
            //     // .AddSource(CustomActivitySources.Name)
            //     .Build();

            using (var activity = CustomActivitySources.Main.StartActivity("ProcessOrder"))
            {
                log.Info($"Received PlaceOrder, OrderId = {message.OrderId}");

                // This is normally where some business logic would occur

                // Uncomment to test throwing transient exceptions
                //if (random.Next(0, 5) == 0)
                //{
                //    throw new Exception("Oops");
                //}

                // Uncomment to test throwing fatal exceptions
                //throw new Exception("BOOM");
                var orderPlaced = new OrderPlaced { OrderId = message.OrderId };
                activity?.SetTag("orderId", message.OrderId);

                log.Info($"Publishing OrderPlaced, OrderId = {message.OrderId}");

                return context.Publish(orderPlaced);
            }
        }
    }
}
