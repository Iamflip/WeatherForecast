using MetricsManagerClient.Client;
using MetricsManagerClient.Model;
using MetricsManagerClient.Request;
using MetricsManagerClient.Responces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MetricsManagerClient.Jobs
{
    [DisallowConcurrentExecution]
    class CpuMetricJob : IJob
    {
        private IMetricsClient _client;
        private AllCpuMetricsApiResponse _responce;
        private DateTimeOffset _UNIX = new DateTime(1970, 01, 01);
        public CpuMetricJob(IMetricsClient client, AllCpuMetricsApiResponse response)
        {
            _client = client;
            _responce = response;
            _responce.Metrics = new List<CpuMetricDto>();
        }
        public Task Execute(IJobExecutionContext context)
        {
            var request = new GetAllCpuMetricsApiRequest();

            if (_responce.Metrics == null || _responce.Metrics.Count == 0)
            {
                request.FromTime = _UNIX;
            }
            else
            {
                request.FromTime = _responce.Metrics[_responce.Metrics.Count - 1].Time;
            }

            request.ToTime = DateTimeOffset.UtcNow;

            var result = _client.GetFromToCpuMetrics(request);

            //_responce.Metrics = new List<CpuMetricDto>();

            if (result.Metrics.Count != 0)
            {
                _responce.Metrics.AddRange(result.Metrics);
            }
            

            return Task.CompletedTask;
        }
    }
}
