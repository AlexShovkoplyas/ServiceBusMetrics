using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ServiceBusMetrics
{
    public class MetricsData
    {
        public List<MetricsRecord> MetricsList { get; set; } = new List<MetricsRecord>();
    }

    public class MetricsRecord
    {
        [Column("Direct/LoadBalancer")]
        public SendOptions? SendOptions { get; set; }

        [Column("Receive mode")]
        public ReceiveMode? ReceiveMode { get; set; }

        [Column("Prefetch count")]
        public int? PrefetchCount { get; set; }

        [Column("Concurrent Calls")]
        public int? ConcurentCount { get; set; }

        [Column("Send/Receive")]
        public string Action { get; set; }

        [Column("Action description")]
        public string ActionDescription { get; set; }

        [Column("Processed messages")]
        public int MessageCount { get; set; }

        [Column("Process duration")]
        public float ProcessDuration { get; set; }

        [Column("Speed (msg/sec)")]
        public float Speed { get => MessageCount / ProcessDuration; }
    }
}
