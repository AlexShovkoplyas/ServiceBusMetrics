using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusMetrics
{
    public class LoadBalancer : IMessageSender
    {
        private List<IMessageSender> senders = new List<IMessageSender>();

        public LoadBalancer(List<IMessageSender> senders)
        {
            this.senders = senders;
        }

        private int _senderPointer;
        private int senderPointer
        {
            get
            {
                if (_senderPointer >= senders.Count)
                {
                    _senderPointer = 0;
                }
                return _senderPointer;
            }
            set
            {
                _senderPointer = value;
            }
        }

        public Task SendAsync(Message message)
        {
            return senders[senderPointer++].SendAsync(message);
        }

        public Task SendAsync(IList<Message> messageList)
        {
            return senders[senderPointer++].SendAsync(messageList);
        }

        #region NotImplemented

        public string ClientId => throw new NotImplementedException();

        public bool IsClosedOrClosing => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public TimeSpan OperationTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ServiceBusConnection ServiceBusConnection => throw new NotImplementedException();

        public IList<ServiceBusPlugin> RegisteredPlugins => throw new NotImplementedException();

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public Task<long> ScheduleMessageAsync(Message message, DateTimeOffset scheduleEnqueueTimeUtc)
        {
            throw new NotImplementedException();
        }

        public Task CancelScheduledMessageAsync(long sequenceNumber)
        {
            throw new NotImplementedException();
        }

        public void RegisterPlugin(ServiceBusPlugin serviceBusPlugin)
        {
            throw new NotImplementedException();
        }

        public void UnregisterPlugin(string serviceBusPluginName)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
