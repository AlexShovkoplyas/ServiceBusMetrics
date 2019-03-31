using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusMetrics
{
    public class LoggingMetrics : IInterceptor
    {
        private TextWriter writer;
        private MetricsData metrics;

        public LoggingMetrics(TextWriter writer, MetricsData metrics)
        {
            this.writer = writer;
            this.metrics = metrics;
        }

        public void Intercept(IInvocation invocation)
        {

            var methodAttribute = invocation.Method.GetCustomAttributes(typeof(MethodAttribute), true)[0] as MethodAttribute;

            Console.WriteLine($"{methodAttribute.ActionType.ToString().ToUpper()} --- {methodAttribute.Description.ToUpper()}");
            Console.WriteLine("Start...");
            var timer = Stopwatch.StartNew();
            invocation.Proceed();

            var returnType = invocation.Method.ReturnType;
            if (returnType == typeof(Task))
            {
                (invocation.ReturnValue as Task).Wait();
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                (invocation.ReturnValue as Task).Wait();
            }

            timer.Stop();
            Console.WriteLine("Stop...");

            var name = $"{ methodAttribute.ActionType }-{ methodAttribute.Description.ToUpper()}";
            var duration = (float)timer.ElapsedMilliseconds / 1000;
            var count = (invocation.InvocationTarget as ICounter).MessagesCount;
            var settings = (invocation.InvocationTarget as ITasksDispatcher).Settings;

            metrics.MetricsList.Add(new MetricsRecord
            {
                SendOptions = settings.SendOptions,
                ReceiveMode = settings.receiveMode,
                PrefetchCount = settings.prefetchCount,
                ConcurentCount = settings.concurentCalls,
                Action = methodAttribute.ActionType.ToString(),
                ActionDescription = methodAttribute.Description,
                MessageCount = count,
                ProcessDuration = duration
            });
        }
    }
}
