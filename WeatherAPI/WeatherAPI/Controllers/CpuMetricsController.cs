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
    [Route("api/metrics/cpu")]
    [ApiController]
    public class CpuMetricsController : ControllerBase
    {
        private readonly ILogger<CpuMetricsController> _logger;
        private IMetricsAgentClient _metricsAgentClient;
        private IRepositoryMM<CpuMetric> _repository;
        private IMapper _mapper;
        private IAgentsRepository<AgentInfo> _agent;

        public CpuMetricsController(ILogger<CpuMetricsController> logger, IMetricsAgentClient metricsAgentClient,
            IRepositoryMM<CpuMetric> repository, IMapper mapper, IAgentsRepository<AgentInfo> agent)
        {
            _agent = agent;
            _mapper = mapper;
            _repository = repository;
            _metricsAgentClient = metricsAgentClient;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в CpuMetricsController");
        }

        [HttpGet("read")]
        public void test()
        {

        }

        /// <summary>
        /// Получает метрики CPU на заданном диапазоне времени от определённого агента
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/cpu/agent/1/from/2020-10-10/to/2021-10-10
        ///
        /// </remarks>
        /// <param name="agentId">айди агента</param>
        /// <param name="fromTime">начальная дата</param>
        /// <param name="toTime">конечная дата</param>
        /// <returns>Список метрик, которые были сохранены в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>
        /// <response code="400">если передали не правильные параетры</response> 
        [HttpGet("agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] int agentId, [FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"Входные данные {agentId} {fromTime} {toTime}");

            var metrics = _repository.GetFromToByAgent(agentId, fromTime, toTime);


            if (metrics == null)
            {
                return Ok();
            }

            var response = new AllCpuMetricsApiResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Получает метрики CPU на заданном диапазоне времени от определённого агента в персентилях
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/cpu/agent/1/from/2020-10-10/to/2021-10-10/percentiles/4
        ///
        /// </remarks>
        /// <param name="agentId">айди агента</param>
        /// <param name="fromTime">начальная дата</param>
        /// <param name="toTime">конечная дата</param>
        /// <param name="percentile">персенитль по которому идёт сравнение</param>
        /// <returns>Список метрик, которые были сохранены в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>
        /// <response code="400">если передали не правильные параетры</response> 
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

            var response = new AllCpuMetricsApiResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            metrics = SortAndDeleteForPercentile(metrics, percentile);

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }

            return Ok($"По перцентилю {percentile} нагрузка не превышает {metrics.Max(metric => metric.Value)}%");
        }

        /// <summary>
        /// Получает метрики CPU на заданном диапазоне времени от всех агентов
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/cpu/cluster/from/2020-10-10/to/2021-10-10
        ///
        /// </remarks>
        /// <param name="fromTime">начальная дата</param>
        /// <param name="toTime">конечная дата</param>
        /// <returns>Список метрик, которые были сохранены в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>
        /// <response code="400">если передали не правильные параетры</response> 
        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAllCluster([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime}");

            var agents = _agent.GetAll();

            if (agents == null)
            {
                return Ok();
            }

            List<CpuMetric> cpuMetrics = new List<CpuMetric>();

            for (int i = 0; i < agents.Count; i++)
            {
                cpuMetrics.AddRange(_repository.GetFromToByAgent(agents[i].AgentId, fromTime, toTime));
            }

            if (cpuMetrics == null)
            {
                return Ok();
            }

            var response = new AllCpuMetricsApiResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            foreach (var metric in cpuMetrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Получает метрики CPU на заданном диапазоне времени от всех агентов с персентилем
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/cpu/cluster/from/2020-10-10/to/2021-10-10/percentiles/4
        ///
        /// </remarks>
        /// <param name="fromTime">начальная дата</param>
        /// <param name="toTime">конечная дата</param>
        /// <param name="percentile">персенитль по которому идёт сравнение</param>
        /// <returns>Список метрик, которые были сохранены в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>
        /// <response code="400">если передали не правильные параетры</response> 
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

            List<CpuMetric> cpuMetrics = new List<CpuMetric>();

            for (int i = 0; i < agents.Count; i++)
            {
                cpuMetrics.AddRange(_repository.GetFromToByAgent(agents[i].AgentId, fromTime, toTime));
            }

            if (cpuMetrics == null)
            {
                return Ok();
            }

            var response = new AllCpuMetricsApiResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            cpuMetrics = (List<CpuMetric>)SortAndDeleteForPercentile(cpuMetrics, percentile);

            foreach (var metric in cpuMetrics)
            {
                response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
            }

            return Ok($"По перцентилю {percentile} нагрузка не превышает {cpuMetrics.Max(metric => metric.Value)}%");
        }

        private IList<CpuMetric> SortAndDeleteForPercentile(IList<CpuMetric> metrics, Percentile percentile)
        {
            var sortedMetrics = (IList<CpuMetric>)metrics.OrderBy(metric => metric.Value);

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
