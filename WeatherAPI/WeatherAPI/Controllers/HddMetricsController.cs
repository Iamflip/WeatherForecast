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
    [Route("api/metrics/hdd")]
    [ApiController]
    public class HddMetricsController : ControllerBase
    {
        private readonly ILogger<HddMetricsController> _logger;
        private IMetricsAgentClient _metricsAgentClient;
        private IRepositoryMM<HddMetric> _repository;
        private IMapper _mapper;
        private IAgentsRepository<AgentInfo> _agent;

        public HddMetricsController(ILogger<HddMetricsController> logger, IMetricsAgentClient metricsAgentClient,
            IRepositoryMM<HddMetric> repository, IMapper mapper, IAgentsRepository<AgentInfo> agent)
        {
            _agent = agent;
            _mapper = mapper;
            _repository = repository;
            _metricsAgentClient = metricsAgentClient;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в HddMetricsController");
        }

        /// <summary>
        /// Получает метрики Hdd на заданном диапазоне времени от определённого агента
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/hdd/agent/1/from/2020-10-10/to/2021-10-10
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

            var response = new AllHddMetricsApiResponse()
            {
                Metrics = new List<HddMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Получает метрики Hdd на заданном диапазоне времени от определённого агента в персентилях
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/hdd/agent/1/from/2020-10-10/to/2021-10-10/percentiles/4
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

            var response = new AllHddMetricsApiResponse()
            {
                Metrics = new List<HddMetricDto>()
            };

            metrics = SortAndDeleteForPercentile(metrics, percentile);

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }

            return Ok($"По перцентилю {percentile} нагрузка не превышает {metrics.Max(metric => metric.Value)}%");
        }

        /// <summary>
        /// Получает метрики Hdd на заданном диапазоне времени от всех агентов
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/hdd/cluster/from/2020-10-10/to/2021-10-10
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

            List<HddMetric> hddMetrics = new List<HddMetric>();

            for (int i = 0; i < agents.Count; i++)
            {
                hddMetrics.AddRange(_repository.GetFromToByAgent(agents[i].AgentId, fromTime, toTime));
            }

            if (hddMetrics == null)
            {
                return Ok();
            }

            var response = new AllHddMetricsApiResponse()
            {
                Metrics = new List<HddMetricDto>()
            };

            foreach (var metric in hddMetrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Получает метрики Hdd на заданном диапазоне времени от всех агентов с персентилем
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET api/metrics/hdd/cluster/from/2020-10-10/to/2021-10-10/percentiles/4
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

            List<HddMetric> hddMetrics = new List<HddMetric>();

            for (int i = 0; i < agents.Count; i++)
            {
                hddMetrics.AddRange(_repository.GetFromToByAgent(agents[i].AgentId, fromTime, toTime));
            }

            if (hddMetrics == null)
            {
                return Ok();
            }

            var response = new AllHddMetricsApiResponse()
            {
                Metrics = new List<HddMetricDto>()
            };

            hddMetrics = (List<HddMetric>)SortAndDeleteForPercentile(hddMetrics, percentile);

            foreach (var metric in hddMetrics)
            {
                response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
            }

            return Ok($"По перцентилю {percentile} нагрузка не превышает {hddMetrics.Max(metric => metric.Value)}%");
        }

        private IList<HddMetric> SortAndDeleteForPercentile(IList<HddMetric> metrics, Percentile percentile)
        {
            var sortedMetrics = (IList<HddMetric>)metrics.OrderBy(metric => metric.Value);

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
