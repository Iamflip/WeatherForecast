using MetricsInfrastucture.Interfaces;
using MetricsManager.Client;
using MetricsManager.Metrics;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetricsManager.Request;

namespace MetricsManager.Jobs
{
    public class RamMetricJob : IJob
    {
        private IRepositoryMM<RamMetric> _metricRepository;
        private IAgentsRepository<AgentInfo> _agentRepository;
        private IMetricsAgentClient _client;
        private DateTimeOffset _UNIX = new DateTime(1970, 01, 01);
        public RamMetricJob(IRepositoryMM<RamMetric> metricRepository, IAgentsRepository<AgentInfo> agentsRepository, IMetricsAgentClient client)
        {
            _metricRepository = metricRepository;
            _agentRepository = agentsRepository;
            _client = client;
        }
        public Task Execute(IJobExecutionContext context)
        {
            IList<AgentInfo> agents = _agentRepository.GetAll();
            var last = _UNIX.AddSeconds(_metricRepository.GetLast().Time.TotalSeconds);

            for (int i = 0; i < agents.Count; i++)
            {
                var temp = new GetAllRamMetricsApiRequest();

                temp.ClientBaseAddress = agents[i].AgentAddress.ToString();
                if (last > _UNIX)
                {
                    temp.FromTime = last;
                }
                else
                {
                    temp.FromTime = _UNIX;
                }
                temp.ToTime = DateTimeOffset.UtcNow;


                var result = _client.GetFromToRamMetrics(temp);

                for (int j = 0; j < result.Metrics.Count; j++)
                {
                    _metricRepository.Create(new RamMetric
                    {
                        AgentId = result.Metrics[i].AgentId,
                        Value = result.Metrics[i].Value,
                        Time = TimeSpan.FromSeconds(result.Metrics[i].Time.ToUnixTimeSeconds())
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}
