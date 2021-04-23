using MetricsManagerClient.Request;
using MetricsManagerClient.Responces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsManagerClient.Client
{
    interface IMetricsClient
    {
        AllCpuMetricsApiResponse GetFromToCpuMetrics(GetAllCpuMetricsApiRequest request);
    }
}
