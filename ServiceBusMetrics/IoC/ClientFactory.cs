using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusMetrics
{
    public static class ClientFactory
    {
        private static string connectionString1 = "Endpoint=sb://testservicebus123456.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=vLw4WVnz8YxJFcCRw5+z33lSxaWwp3eHMzYyxV+TNj8=";
        private static string connectionString2 = "Endpoint=sb://testservicebus234567.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=fyCG3oZT03niJaUjrvLsxRrh6txPo3C1DcK4vbVClTk=";

        private static string queueName = "queue1";

        public static MessageSender CreateSender()
        {
            return new MessageSender(connectionString1, queueName);
        }

        public static MessageReceiver CreateReceiver(BusSettings settings)
        {
            var receiver = settings.receiveMode.HasValue
                    ? new MessageReceiver(connectionString1, queueName, receiveMode: settings.receiveMode.Value)
                    : new MessageReceiver(connectionString1, queueName);
            if (settings.prefetchCount.HasValue)
            {
                receiver.PrefetchCount = settings.prefetchCount.Value;
            }
            return receiver;
        }

        public static LoadBalancer CreateLoadBalancer()
        {
            var connections = new string[] { connectionString1, connectionString2 };
            var senders = new List<IMessageSender>();
            foreach (var connection in connections)
            {
                senders.Add(new MessageSender(connection, queueName));
            }
            return new LoadBalancer(senders);
        }            
    }
}
