using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MetricsAgent.Metric;
using MetricsAgent.Responses;
using AutoMapper;
using MetricsInfrastucture.Interfaces;
using System;
using System.Collections.Generic;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]
    public class RamMetricsController : ControllerBase
    {
        private readonly ILogger<RamMetricsController> _logger;
        private IRepository<RamMetric> _repository;
        private IMapper _mapper;

        public RamMetricsController (ILogger<RamMetricsController> logger, IRepository<RamMetric> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в RamMetricsController");
        }

        [HttpGet("available")]
        public IActionResult GetMetricsFromAgent()
        {
            var metric = _repository.GetLast();

            if (metric == null)
            {
                return Ok();
            }

            return Ok(_mapper.Map<RamMetricDto>(metric));
        }

        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime}");

            var metrics = _repository.GetFromTo(fromTime, toTime);

            if (metrics == null)
            {
                return Ok();
            }

            var response = new AllRamMetricsResponse()
            {
                Metrics = new List<RamMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(_mapper.Map<RamMetricDto>(metric));
            }

            return Ok(response);
        }
    }
}
