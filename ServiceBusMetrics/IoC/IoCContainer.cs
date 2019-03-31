using Autofac;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusMetrics
{
    public class IoCContainer
    {
        public IContainer Buid()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MetricsData>().AsSelf().InstancePerLifetimeScope();
            builder.Register(b => new LoggingMetrics(Console.Out, b.Resolve<MetricsData>()));

            builder.Register<MessageSender>(c => ClientFactory.CreateSender()).As<IMessageSender>().Keyed<IMessageSender>(SendOptions.Direct);
            builder.Register<LoadBalancer>(c => ClientFactory.CreateLoadBalancer()).As<IMessageSender>().Keyed<IMessageSender>(SendOptions.LoadBalancer);

            builder.Register<MessageReceiver>((c,p) => ClientFactory.CreateReceiver(p.TypedAs<BusSettings>())).As<IMessageReceiver>();

            builder.RegisterType<TasksDispatcher>().As<ITasksDispatcher>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(LoggingMetrics));

            builder.RegisterType<MetricsWriter>().AsSelf();

            return builder.Build();
        }

        public static ITasksDispatcher CreateTasksDispatcher(ILifetimeScope container, BusSettings busSettings)
        {
            return container.Resolve<ITasksDispatcher>(
                new TypedParameter(
                    typeof(BusSettings),
                    busSettings),
                new ResolvedParameter(
                   (pi, ctx) => pi.ParameterType == typeof(IMessageSender),
                   (pi, ctx) => ctx.ResolveKeyed<IMessageSender>(busSettings.SendOptions ?? SendOptions.Direct)),
                new ResolvedParameter(
                   (pi, ctx) => pi.ParameterType == typeof(IMessageReceiver),
                   (pi, ctx) => ctx.Resolve<IMessageReceiver>(new TypedParameter(typeof(BusSettings), busSettings))));
        }
    }
}
