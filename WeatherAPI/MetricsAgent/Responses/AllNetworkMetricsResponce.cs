using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Responses
{
    public class AllNetworkMetricsResponce
    {
        public List<NetworkMetricDto> Metrics { get; set; }
    }
}
