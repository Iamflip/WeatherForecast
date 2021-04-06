using MetricsAgent.DAL;
using Quartz;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;
using MetricsAgent.Metric;

namespace MetricsAgent.Jobs
{
    public class HddMetricJob : IJob
    {
        private HddMetricsRepository _repository;

        private PerformanceCounter _hddCounter;

        public HddMetricJob()
        {
            _repository = new HddMetricsRepository();
            _hddCounter = new PerformanceCounter("LogicalDisk", "Free Megabytes", "_Total");
        }

        public Task Execute(IJobExecutionContext context)
        {
            var hddAvailable = Convert.ToInt32(_hddCounter.NextValue());

            // узнаем когда мы сняли значение метрики.
            var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // теперь можно записать что-то при помощи репозитория

            _repository.Create(new HddMetric { Time = time, Value = hddAvailable });

            return Task.CompletedTask;
        }
    }
}
