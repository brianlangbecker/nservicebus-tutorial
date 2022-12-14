using NServiceBus;
using NServiceBus.Logging;

namespace Messages
{
    public class PlaceOrder : ICommand
    {
        public string OrderId { get; set; }
    }
}
