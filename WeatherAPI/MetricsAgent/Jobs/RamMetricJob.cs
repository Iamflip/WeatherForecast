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
    public class RamMetricJob : IJob
    {
        private IRepository<RamMetric> _repository;

        private PerformanceCounter _ramCounter;

        public RamMetricJob(IRepository<RamMetric> repository)
        {
            _repository = repository;
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
