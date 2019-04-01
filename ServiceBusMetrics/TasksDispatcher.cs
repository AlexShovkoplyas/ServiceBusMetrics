using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System.Threading.Tasks;

namespace ServiceBusMetrics
{
    public partial class TasksDispatcher : ITasksDispatcher
    {
        private IMessageSender sender;
        private IMessageReceiver receiver;
        private Postman messagePostman;

        public TasksDispatcher(IMessageSender sender, IMessageReceiver receiver, BusSettings settings)
        {
            this.sender = sender;
            this.receiver = receiver;
            Settings = settings;
            messagePostman = new Postman(sender, receiver);
        }

        public BusSettings Settings { get; private set; }

        public int MessagesCount { get; set; }

        public async Task ReceiveMessagesWithHandler(int duration)
        {
            messagePostman.StartReceivingWithHandler(
                Settings.concurentCalls.HasValue ? Settings.concurentCalls.Value : 1, 
                Settings.receiveMode == ReceiveMode.ReceiveAndDelete);

            await Task.Delay(duration * 1000)
                .ContinueWith((t) => messagePostman.StopReceiving());
            MessagesCount = messagePostman.MessagesCount;
        }

        public async Task ReceiveMessagesOneByOne(int messagesCount)
        {
            await messagePostman.ReceiveMessagesOneByOne(messagesCount);
            MessagesCount = messagesCount;
        }

        public async Task ReceiveMessagesInBunch(int messagesCount)
        {
            await messagePostman.ReceiveMessagesOneByOne(messagesCount);
            MessagesCount = messagesCount;
        }

        public async Task SendMessagesOneByOne(int messagesCount)
        {
            await messagePostman.SendMessagesOneByOne(messagesCount);
            MessagesCount = messagesCount;
        }

        public async Task SendMessagesSeparately(int messagesCount)
        {
            System.Console.WriteLine("Main : " + messagePostman.SendMessagesSeparately(messagesCount).Status);
            await messagePostman.SendMessagesSeparately(messagesCount);
            MessagesCount = messagesCount;
        }

        public async Task SendMessagesSeparatelyFifo(int messagesCount)
        {
            await messagePostman.SendMessagesSeparatelyFifo(messagesCount);
            MessagesCount = messagesCount;
        }

        public async Task SendMessagesInBunch(int messagesCount)
        {
            await messagePostman.SendMessagesSeparately(messagesCount);
            MessagesCount = messagesCount;
        }
    }
}