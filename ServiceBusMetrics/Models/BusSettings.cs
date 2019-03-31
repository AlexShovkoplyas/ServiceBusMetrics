using Microsoft.Azure.ServiceBus;

namespace ServiceBusMetrics
{
    public class BusSettings
    {
        public SendOptions? SendOptions { get; set; }

        public int? prefetchCount { get; set; }

        public int? concurentCalls { get; set; }

        public ReceiveMode? receiveMode { get; set; }
    }
}
