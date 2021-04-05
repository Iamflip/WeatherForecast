using MetricsInfrastucture.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MetricsAgent.Responses;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/dotnet")]
    [ApiController]
    public class DotNetMetricsController : ControllerBase
    {
        private readonly ILogger<DotNetMetricsController> _logger;
        private IRepository<DotNetMetric> _repository;
        private DateTime UNIX = new DateTime(1970, 01, 01);

        public DotNetMetricsController(ILogger<DotNetMetricsController> logger, IRepository<DotNetMetric> repository)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в DotNetMetricsController");
        }

        [HttpGet("errors-count/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime}");

            var metrics = _repository.GetFromTo(fromTime, toTime);

            if (metrics == null)
            {
                return Ok();
            }

            var response = new AllDotNetMetricsResponse()
            {
                Metrics = new List<DotNetMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(new DotNetMetricDto
                { Time = UNIX.AddSeconds(metric.Time.TotalSeconds), Value = metric.Value, Id = metric.Id });
            }

            return Ok(response);
        }
    }
}
