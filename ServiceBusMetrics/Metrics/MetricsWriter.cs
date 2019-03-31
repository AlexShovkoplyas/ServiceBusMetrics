using ConsoleTableExt;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ServiceBusMetrics
{
    public class MetricsWriter
    {
        private string[] columnNames = typeof(MetricsRecord).GetProperties()
            .Select(p => ((ColumnAttribute)p.GetCustomAttributes(true)[0]).Name).ToArray();

        private StringBuilder output = new StringBuilder();

        public void WriteData(string header, MetricsData metrics)
        {
            if (metrics.MetricsList.Any())
            {
                output.Append($"***** {header.ToUpper()} *****\n");
                output.Append(ConsoleTableBuilder
                   .From(metrics.MetricsList)
                   .WithFormat(ConsoleTableBuilderFormat.Alternative)
                   .WithColumn(columnNames)
                   .Export());
                output.Append("\n");
            }
        }

        public override string ToString()
        {
            return output.ToString();
        }
    }
}
