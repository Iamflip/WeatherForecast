using MetricsAgent.DAL;
using Quartz;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;
using MetricsAgent.Metric;
using MetricsInfrastucture.Interfaces;

namespace MetricsAgent.Jobs
{
    public class HddMetricJob : IJob
    {
        private IRepository<HddMetric> _repository;

        private PerformanceCounter _hddCounter;

        public HddMetricJob(IRepository<HddMetric> repository)
        {
            _repository = repository;
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
