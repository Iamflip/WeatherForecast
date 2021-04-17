using MetricsAgent.DAL;
using MetricsInfrastucture.Interfaces;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
    public class DotNetMetricJob : IJob
    {
        private IRepository<DotNetMetric> _repository;

        private PerformanceCounter _dotnetCounter;

        public DotNetMetricJob(IRepository<DotNetMetric> repository)
        {
            _repository = repository;
            _dotnetCounter = new PerformanceCounter(".NET CLR Memory", "% Time in GC", "_Global_");
        }

        public Task Execute(IJobExecutionContext context)
        {
            // получаем значение занятости CPU
            var heapSize = Convert.ToInt32(_dotnetCounter.NextValue());

            // узнаем когда мы сняли значение метрики.
            var time = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // теперь можно записать что-то при помощи репозитория

            _repository.Create(new DotNetMetric { Time = time, Value = heapSize });

            return Task.CompletedTask;
        }
    }
}
