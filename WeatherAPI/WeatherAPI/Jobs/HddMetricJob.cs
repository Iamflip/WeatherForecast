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
    public class HddMetricJob : IJob
    {
        private IRepositoryMM<HddMetric> _metricRepository;
        private IAgentsRepository<AgentInfo> _agentRepository;
        private IMetricsAgentClient _client;
        private DateTimeOffset _UNIX = new DateTime(1970, 01, 01);
        public HddMetricJob(IRepositoryMM<HddMetric> metricRepository, IAgentsRepository<AgentInfo> agentsRepository, IMetricsAgentClient client)
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
                var temp = new GetAllHddMetricsApiRequest();

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


                var result = _client.GetFromToHddMetrics(temp);

                for (int j = 0; j < result.Metrics.Count; j++)
                {
                    _metricRepository.Create(new HddMetric
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
