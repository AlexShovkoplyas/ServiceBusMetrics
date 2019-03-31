using Autofac;
using System;
using System.Threading.Tasks;
using System.IO;

namespace ServiceBusMetrics
{
    class Program
    {        
        static async Task Main(string[] args)
        {
            var container = new IoCContainer().Buid();
            var metricsWriter = container.Resolve<MetricsWriter>();

            var scenarios = new Scenarios();

            foreach (var scenario in scenarios.ScenariosDictionary)
            {
                using (var scope = container.BeginLifetimeScope())
                {
                    var metricsData = scope.Resolve<MetricsData>();
                    await scenario.Value(scope);
                    metricsWriter.WriteData(scenario.Key, metricsData);
                }
            }

            WriteToFile(metricsWriter);
            WriteToConsole(metricsWriter);

            Console.WriteLine("Finished...");
            Console.ReadLine();
        }

        private static void WriteToConsole(MetricsWriter metricsWriter)
        {
            Console.WriteLine(metricsWriter.ToString());
        }

        private static void WriteToFile(MetricsWriter metricsWriter)
        {
            Directory.CreateDirectory("output");

            var path1 = $"output/results-{DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ss")}.txt";
            var path2 = $"output/results-Last.txt";
            using (var tw = new StreamWriter(path1, true))
            {
                tw.WriteLine(metricsWriter.ToString());
            }

            FileInfo Sourcefile = new FileInfo(path1);
            Sourcefile.CopyTo(path2, true);

            //File.Replace(path1, path2, path2 + ".bac");

        }
    }
}
