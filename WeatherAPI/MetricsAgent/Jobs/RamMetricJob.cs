using MetricsAgent.DAL;
using Quartz;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading.Tasks;
using MetricsAgent.Metric;

namespace MetricsAgent.Jobs
{
    public class RamMetricJob : IJob
    {
        private RamMetricsRepository _repository;

        private PerformanceCounter _ramCounter;

        public RamMetricJob()
        {
            _repository = new RamMetricsRepository();
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public Task Execute(IJobExecutionContext context)
        {
            var ramAvailable = Convert.ToInt32(_ramCounter.NextValue());

            // узнаем когда мы сняли значение метрики.
            var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // теперь можно записать что-то при помощи репозитория

            _repository.Create(new RamMetric { Time = time, Value = ramAvailable });

            return Task.CompletedTask;
        }
    }
}
