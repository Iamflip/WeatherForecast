using MetricsAgent.DAL;
using MetricsAgent.Metric;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
    public class NetworkMetricJob : IJob
    {
        private IRepository<NetworkMetric> _repository;

        private PerformanceCounter _networkCounter;

        public NetworkMetricJob(IRepository<NetworkMetric> repository)
        {
            _repository = repository;
        }

        public Task Execute(IJobExecutionContext context)
        {
            int network = 0;

            var perfomanceCounterCategory = new PerformanceCounterCategory("Network Interface");
            string[] instances = perfomanceCounterCategory.GetInstanceNames();

            for (int i = 0; i < instances.Length; i++)
            {
                _networkCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instances[i]);
                network += Convert.ToInt32(_networkCounter.NextValue());
            }
            

            // узнаем когда мы сняли значение метрики.
            var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // теперь можно записать что-то при помощи репозитория

            _repository.Create(new NetworkMetric { Time = time, Value = network });

            return Task.CompletedTask;
        }
    }
}
