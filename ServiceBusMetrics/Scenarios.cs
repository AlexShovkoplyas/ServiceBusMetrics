using Autofac;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusMetrics
{
    public class Scenarios
    {
        public ILifetimeScope container;

        public Scenarios()
        {
            BuildScenarios();
        }

        public Dictionary<string, Func<ILifetimeScope,Task>> ScenariosDictionary { get; private set; }

        private void BuildScenarios()
        {
            var sendCount = 1500;
            var receiveCount = 300;
            var duration = 10;

            BusSettings busSettings;
            ITasksDispatcher dispatcher;

            //ScenariosDictionary = new Dictionary<string, Func<ILifetimeScope, Task>>
            //{
            //    {
            //        "Compare send methods (without/with Load Balancing)",
            //        async (container) =>
            //        {
            //            busSettings = new BusSettings();
            //            dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
            //            await dispatcher.SendMessagesSeparatelyFifo(3);
            //        }
            //    }
            //};

            ScenariosDictionary = new Dictionary<string, Func<ILifetimeScope, Task>>
            {
                {
                    "Compare send methods (without/with Load Balancing)",
                    async (container) =>
                    {
                        busSettings = new BusSettings();
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.SendMessagesOneByOne(sendCount);
                        await dispatcher.SendMessagesSeparately(sendCount);
                        await dispatcher.SendMessagesInBunch(sendCount);
                        busSettings = new BusSettings(){ SendOptions = SendOptions.LoadBalancer };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.SendMessagesOneByOne(sendCount);
                        await dispatcher.SendMessagesSeparately(sendCount);
                        await dispatcher.SendMessagesInBunch(sendCount);
                    }
                },
                {
                    "Compare PeekLock vs ReceiveAndDelete",
                    async (container) =>
                    {
                        busSettings = new BusSettings { prefetchCount = 6, concurentCalls = 6, receiveMode = ReceiveMode.PeekLock };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesOneByOne(receiveCount);
                        await dispatcher.ReceiveMessagesInBunch(receiveCount);
                        //await dispatcher.ReceiveMessagesWithHandler(duration);
                        busSettings = new BusSettings { prefetchCount = 6, concurentCalls = 6, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesOneByOne(receiveCount);
                        await dispatcher.ReceiveMessagesInBunch(receiveCount);
                        //await dispatcher.ReceiveMessagesWithHandler(duration);
                    }
                },
                {
                    "Compare Prefetch count",
                    async (container) =>
                    {
                        busSettings = new BusSettings { prefetchCount = 0,  receiveMode = ReceiveMode.PeekLock };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesOneByOne(receiveCount);
                        await dispatcher.ReceiveMessagesInBunch(receiveCount);
                        busSettings = new BusSettings { prefetchCount = 1,  receiveMode = ReceiveMode.PeekLock };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesOneByOne(receiveCount);
                        await dispatcher.ReceiveMessagesInBunch(receiveCount);
                        busSettings = new BusSettings { prefetchCount = 10,  receiveMode = ReceiveMode.PeekLock };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesOneByOne(receiveCount);
                        await dispatcher.ReceiveMessagesInBunch(receiveCount);
                        busSettings = new BusSettings { prefetchCount = 100,  receiveMode = ReceiveMode.PeekLock };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesOneByOne(receiveCount);
                        await dispatcher.ReceiveMessagesInBunch(receiveCount);
                    }
                },
                {
                    "Compare receving messages by handler with different settings",
                    async (container) =>
                    {
                        busSettings = new BusSettings { prefetchCount = 1, concurentCalls = 1, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesWithHandler(duration);
                        busSettings = new BusSettings { prefetchCount = 1, concurentCalls = 6, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesWithHandler(duration);
                        busSettings = new BusSettings { prefetchCount = 6, concurentCalls = 1, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesWithHandler(duration);
                        busSettings = new BusSettings { prefetchCount = 6, concurentCalls = 6, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesWithHandler(duration);
                        busSettings = new BusSettings { prefetchCount = 6, concurentCalls = 50, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesWithHandler(duration);
                        busSettings = new BusSettings { prefetchCount = 50, concurentCalls = 6, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesWithHandler(duration);
                        busSettings = new BusSettings { prefetchCount = 100, concurentCalls = 100, receiveMode = ReceiveMode.ReceiveAndDelete };
                        dispatcher = IoCContainer.CreateTasksDispatcher(container, busSettings);
                        await dispatcher.ReceiveMessagesWithHandler(duration);
                    }
                },

            //    //TODO: Handler without messages autocomplete throws exceptions !!!!
            //};
        }
    }
}
