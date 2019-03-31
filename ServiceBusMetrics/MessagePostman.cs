using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusMetrics
{
    public class MessagePostman
    {
        private IMessageSender sender;
        private IMessageReceiver receiver;

        public MessagePostman(IMessageSender sender, IMessageReceiver receiver)
        {
            this.sender = sender;
            this.receiver = receiver;
        }

        public int MessagesCount { get; set; } = 0;

        public async Task SendMessagesOneByOne(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await sender.SendAsync(new Message(Encoding.UTF8.GetBytes(i.ToString())));
            }
        }

        public Task SendMessagesSeparately(int count)
        {
            var messages = Enumerable.Range(0, count)
                .Select(i => new Message(Encoding.UTF8.GetBytes(i.ToString())));
            var tasks = messages.Select(msg => sender.SendAsync(msg));
            return Task.WhenAll(tasks);
        }

        public Task SendMessagesInBunch(int count)
        {
            var messages = Enumerable.Range(0, count)
                .Select(i => new Message(Encoding.UTF8.GetBytes(i.ToString()))).ToList();
            return sender.SendAsync(messages);
        }

        public async Task ReceiveMessagesOneByOne(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var message = await receiver.ReceiveAsync();
                if (receiver.ReceiveMode == ReceiveMode.PeekLock)
                {
                    await receiver.CompleteAsync(message.SystemProperties.LockToken);
                }                
            }
        }

        public async Task ReceiveMessagesInBunch(int count)
        {
            var messages = await receiver.ReceiveAsync(count);

            for (int i = 0; i < count; i++)
            {
                if (receiver.ReceiveMode == ReceiveMode.PeekLock)
                {
                    await receiver.CompleteAsync(messages[i].SystemProperties.LockToken);
                }
            }
        }

        public void StartReceivingWithHandler(int concurrentCalls, bool autoComplete)
        {
            var options = new MessageHandlerOptions((args) => Task.CompletedTask)
            {
                MaxConcurrentCalls = concurrentCalls,
                AutoComplete = autoComplete
            };
            receiver.RegisterMessageHandler(MessageHandler, options);
        }

        public async Task StopReceiving()
        {
            var ps = receiver.RegisteredPlugins;
            await receiver.CloseAsync();
            
        }

        private async Task MessageHandler(Message msg, CancellationToken token)
        {
            MessagesCount++;
            if (receiver.ReceiveMode == ReceiveMode.PeekLock)
            {
                await receiver.CompleteAsync(msg.SystemProperties.LockToken);
            }            
        }
    }
}
