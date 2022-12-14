using NServiceBus;
using NServiceBus.Logging;

namespace Messages
{
    public class OrderPlaced : IEvent
    {
        public string OrderId { get; set; }
    }
}
