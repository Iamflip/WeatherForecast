﻿using MetricsInfrastucture.Interfaces;
using MetricsManager.Client;
using MetricsManager.Metrics;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetricsManager.Request;

namespace MetricsManager.Jobs
{
    public class CpuMetricJob : IJob
    {
        private IRepositoryMM<CpuMetric> _metricRepository;
        private IAgentsRepository<AgentInfo> _agentRepository;
        private IMetricsAgentClient _client;
        public CpuMetricJob(IRepositoryMM<CpuMetric> metricRepository, IAgentsRepository<AgentInfo> agentsRepository, IMetricsAgentClient client)
        {
            _metricRepository = metricRepository;
            _agentRepository = agentsRepository;
            _client = client;
        }
        public Task Execute(IJobExecutionContext context)
        {
            IList<AgentInfo> agents = _agentRepository.GetAll();

            List<DateTimeOffset> last = new List<DateTimeOffset>();

            for (int i = 0; i < agents.Count; i++)
            {
                last.Add(DateTimeOffset.FromUnixTimeSeconds((long)_metricRepository.GetLastFromAgent(agents[i].AgentId).Time.TotalSeconds));
            }

            for (int i = 0; i < agents.Count; i++)
            {
                var request = new GetAllCpuMetricsApiRequest();

                request.ClientBaseAddress = agents[i].AgentAddress;
                if (last[i] > DateTimeOffset.UnixEpoch)
                {
                    request.FromTime = last[i];
                }
                else
                {
                    request.FromTime = DateTimeOffset.UnixEpoch;
                }
                request.ToTime = DateTimeOffset.UtcNow;

                var result = _client.GetFromToCpuMetrics(request);

                for (int j = 0; j < result.Metrics.Count; j++)
                {
                    _metricRepository.Create(new CpuMetric
                    {
                        AgentId = result.Metrics[j].AgentId,
                        Value = result.Metrics[j].Value,
                        Time = TimeSpan.FromSeconds(result.Metrics[j].Time.ToUnixTimeSeconds())
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}
