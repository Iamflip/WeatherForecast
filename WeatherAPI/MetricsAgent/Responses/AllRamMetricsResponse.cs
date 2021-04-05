using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Responses
{
    public class AllRamMetricsResponse
    {
        public List<RamMetricDto> Metrics { get; set; }
    }
}
