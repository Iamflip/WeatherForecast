using MetricsManagerClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsManagerClient.Responces
{
    public class AllCpuMetricsApiResponse
    {
        public List<CpuMetricDto> Metrics { get; set; }
    }
}
