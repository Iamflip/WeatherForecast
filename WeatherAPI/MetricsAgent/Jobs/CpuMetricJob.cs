using MetricsAgent.DAL;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
    public class CpuMetricJob : IJob
    {
        private IRepository<CpuMetric> _repository;

        private PerformanceCounter _cpuCounter;

        public CpuMetricJob(IRepository<CpuMetric> repository)
        {
            _repository = repository;
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        public Task Execute(IJobExecutionContext context)
        {
            // получаем значение занятости CPU
            var cpuUsageInPercents = Convert.ToInt32(_cpuCounter.NextValue());

            // узнаем когда мы сняли значение метрики.
            var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // теперь можно записать что-то при помощи репозитория

            _repository.Create(new CpuMetric { Time = time, Value = cpuUsageInPercents });

            return Task.CompletedTask;
        }
    }
}
