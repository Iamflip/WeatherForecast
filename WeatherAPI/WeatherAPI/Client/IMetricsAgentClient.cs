using MetricsManager.Request;
using MetricsManager.Responses;

namespace MetricsManager.Client
{
    public interface IMetricsAgentClient
    {
        AllRamMetricsApiResponse GetFromToRamMetrics(GetAllRamMetricsApiRequest request);

        AllHddMetricsApiResponse GetFromToHddMetrics(GetAllHddMetricsApiRequest request);

        DotNetMetricsApiResponse GetFromToDotNetMetrics(GetDotNetHeapMetrisApiRequest request);

        AllCpuMetricsApiResponse GetFromToCpuMetrics(GetAllCpuMetricsApiRequest request);

        AllNetworkMetricsApiResponce GetFromToNetworkMetrics(GetAllNetworkMetricsApiRequest request);
    }
}
