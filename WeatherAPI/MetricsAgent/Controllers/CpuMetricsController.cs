using MetricsInfrastucture.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Data.SQLite;
using MetricsAgent.Responses;
using AutoMapper;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/cpu")]
    [ApiController]
    public class CpuMetricsController : ControllerBase
    {
        private readonly ILogger<CpuMetricsController> _logger;
        private IRepository<CpuMetric> _repository;
        private DateTime _UNIX = new DateTime(1970, 01, 01);

        public CpuMetricsController(ILogger<CpuMetricsController> logger, IRepository<CpuMetric> repository)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в CpuMetricsController");
        }

        [HttpGet("from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromAgent([FromRoute] DateTime fromTime, [FromRoute] DateTime toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime} {percentile}");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<CpuMetric, CpuMetricDto>());
            var m = config.CreateMapper();

            var metrics = _repository.GetFromTo(fromTime, toTime);

            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            metrics = SortAndDeleteForPercentile(metrics, percentile);

            foreach (var metric in metrics)
            {
                response.Metrics.Add(m.Map<CpuMetricDto>(metric));
            }

            return Ok($"По перцентилю {percentile} нагрузка не превышает {metrics.Max(metric => metric.Value)}%");
        }

        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime}");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<CpuMetric, CpuMetricDto>());
            var m = config.CreateMapper();

            var metrics = _repository.GetFromTo(fromTime, toTime);

            if (metrics == null)
            {
                return Ok();
            }

            var response = new AllCpuMetricsResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(m.Map<CpuMetricDto>(metric));
            }

            return Ok(response);
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

