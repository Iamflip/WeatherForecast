using Microsoft.AspNetCore.Mvc;
using System;
using MetricsInfrastucture.Enums;
using Microsoft.Extensions.Logging;
using MetricsManager.Client;
using MetricsManager.DAL;
using MetricsManager.Responses;
using MetricsManager.Models;
using System.Collections.Generic;
using AutoMapper;
using MetricsManager.Metrics;
using System.Linq;
using MetricsInfrastucture.Interfaces;

namespace MetricsManager.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]
    public class RamMetricsController : ControllerBase
    {
        private readonly ILogger<RamMetricsController> _logger;
        private IMetricsAgentClient _metricsAgentClient;
        private IRepositoryMM<RamMetric> _repository;
        private IMapper _mapper;
        private IAgentsRepository<AgentInfo> _agent;

        public RamMetricsController(ILogger<RamMetricsController> logger, IMetricsAgentClient metricsAgentClient,
            IRepositoryMM<RamMetric> repository, IMapper mapper, IAgentsRepository<AgentInfo> agent)
        {
            _agent = agent;
            _mapper = mapper;
            _repository = repository;
            _metricsAgentClient = metricsAgentClient;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в RamMetricsController");
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"Входные данные {agentId} {fromTime} {toTime}");

            var metrics = _repository.GetFromToByAgent(agentId, fromTime, toTime);

            if (metrics == null)
            {
                return Ok();
            }

            var response = new AllRamMetricsApiResponse()
            {
                Metrics = new List<RamMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            return Ok(response);
        }

        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogInformation($"Входные данные {agentId} {fromTime} {toTime} {percentile}");

            var metrics = _repository.GetFromToByAgent(agentId, fromTime, toTime);

            if (metrics == null)
            {
                return Ok();
            }

            var response = new AllRamMetricsApiResponse()
            {
                Metrics = new List<RamMetricDto>()
            };

            metrics = SortAndDeleteForPercentile(metrics, percentile);

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            return Ok($"По перцентилю {percentile} нагрузка не превышает {metrics.Max(metric => metric.Value)}%");
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime}");

            var agents = _agent.GetAll();

            if (agents == null)
            {
                return Ok();
            }

            List<RamMetric> ramMetrics = new List<RamMetric>();

            for (int i = 0; i < agents.Count; i++)
            {
                ramMetrics.AddRange(_repository.GetFromToByAgent(agents[i].AgentId, fromTime, toTime));
            }

            if (ramMetrics == null)
            {
                return Ok();
            }

            var response = new AllRamMetricsApiResponse()
            {
                Metrics = new List<RamMetricDto>()
            };

            foreach (var metric in ramMetrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }   

            return Ok(response);
        }

        [HttpGet("cluster/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime} {percentile}");

            var agents = _agent.GetAll();

            if (agents == null)
            {
                return Ok();
            }

            List<RamMetric> ramMetrics = new List<RamMetric>();

            for (int i = 0; i < agents.Count; i++)
            {
                ramMetrics.AddRange(_repository.GetFromToByAgent(agents[i].AgentId, fromTime, toTime));
            }

            if (ramMetrics == null)
            {
                return Ok();
            }

            var response = new AllRamMetricsApiResponse()
            {
                Metrics = new List<RamMetricDto>()
            };

            ramMetrics = (List<RamMetric>)SortAndDeleteForPercentile(ramMetrics, percentile);

            foreach (var metric in ramMetrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            return Ok($"По перцентилю {percentile} нагрузка не превышает {ramMetrics.Max(metric => metric.Value)}%");
        }

        private IList<RamMetric> SortAndDeleteForPercentile(IList<RamMetric> metrics, Percentile percentile)
        {
            var sortedMetrics = (IList<RamMetric>)metrics.OrderBy(metric => metric.Value);

            switch (percentile)
            {
                case Percentile.Median:
                    break;
                case Percentile.P75:
                    if (sortedMetrics.Count >= 4 && sortedMetrics.Count != 0)
                    {
                        int deleteCount = (int)(sortedMetrics.Count * 0.25);
                        sortedMetrics.RemoveAt(sortedMetrics.Count - 1 - deleteCount);
                        return sortedMetrics;
                    }
                    else
                    {
                        return sortedMetrics;
                    }
                case Percentile.P90:
                    if (sortedMetrics.Count >= 10 && sortedMetrics.Count != 0)
                    {
                        int deleteCount = (int)(sortedMetrics.Count * 0.10);
                        sortedMetrics.RemoveAt(sortedMetrics.Count - 1 - deleteCount);
                        return sortedMetrics;
                    }
                    else
                    {
                        return sortedMetrics;
                    }
                case Percentile.P95:
                    if (sortedMetrics.Count >= 20 && sortedMetrics.Count != 0)
                    {
                        int deleteCount = (int)(sortedMetrics.Count * 0.05);
                        sortedMetrics.RemoveAt(sortedMetrics.Count - 1 - deleteCount);
                        return sortedMetrics;
                    }
                    else
                    {
                        return sortedMetrics;
                    }
                case Percentile.P99:
                    if (sortedMetrics.Count >= 100 && sortedMetrics.Count != 0)
                    {
                        int deleteCount = (int)(sortedMetrics.Count * 0.01);
                        sortedMetrics.RemoveAt(sortedMetrics.Count - 1 - deleteCount);
                        return sortedMetrics;
                    }
                    else
                    {
                        return sortedMetrics;
                    }
                default:
                    return sortedMetrics;
            }
            return sortedMetrics;
        }
    }
}
