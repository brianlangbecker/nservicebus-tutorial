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

namespace Billing
{
    public class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        static readonly ILog log = LogManager.GetLogger<OrderPlacedHandler>();

        static class CustomActivitySources
        {
            public const string Name = "Example.Billing";
            public static ActivitySource Main = new ActivitySource(Name);
        }

        public Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            using (var activity = CustomActivitySources.Main.StartActivity("BillOrder"))
            {
                activity?.SetTag("orderId", message.OrderId);
                log.Info($"Billing has received OrderPlaced, OrderId = {message.OrderId}");
                return Task.CompletedTask;
            }
        }
    }
}
