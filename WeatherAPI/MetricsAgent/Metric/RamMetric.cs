using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Metric
{
    public class RamMetric
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public TimeSpan Time { get; set; }
    }
}
