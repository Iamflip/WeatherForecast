using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using MetricsAgent.Metric;
using MetricsAgent.Responses;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/hdd")]
    [ApiController]
    public class HddMetricsController : ControllerBase
    {
        private readonly ILogger<HddMetricsController> _logger;
        private IRepository<HddMetric> _repository;
        private DateTime UNIX = new DateTime(1970, 01, 01);

        public HddMetricsController(ILogger<HddMetricsController> logger, IRepository<HddMetric> repository)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в HddMetricsController");
        }

        [HttpGet("left")]
        public IActionResult GetMetricsFromAgent()
        {
            var metric = _repository.GetLast();

            if (metric == null)
            {
                return Ok();
            }

            var responce = new HddMetricDto
            {
                Id = metric.Id,
                Value = metric.Value,
                Time = UNIX.AddSeconds(metric.Time.TotalSeconds)
            };

            return Ok(responce);
        }
    }
}
