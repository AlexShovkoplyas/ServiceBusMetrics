using System.Threading.Tasks;

namespace ServiceBusMetrics
{
    public interface ITasksDispatcher : ICounter
    {
        BusSettings Settings { get; }

        [Method("One by one", ActionType.Receive)]
        Task ReceiveMessagesOneByOne(int messagesCount);

        [Method("In bunch", ActionType.Receive)]
        Task ReceiveMessagesInBunch(int messagesCount);
        
        [Method("With handler", ActionType.Receive)]
        Task ReceiveMessagesWithHandler(int duration);

        [Method("One by one", ActionType.Send)]
        Task SendMessagesOneByOne(int messagesCount);

        [Method("Separately (in one Task)", ActionType.Send)]
        Task SendMessagesSeparately(int messagesCount);

        [Method("Separately (in one Task FIFO)", ActionType.Send)]
        Task SendMessagesSeparatelyFifo(int messagesCount);

        [Method("In bunch", ActionType.Send)]
        Task SendMessagesInBunch(int messagesCount);
    }
}