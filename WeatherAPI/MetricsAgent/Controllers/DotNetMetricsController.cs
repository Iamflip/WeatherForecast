using MetricsInfrastucture.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MetricsAgent.Responses;
using AutoMapper;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/dotnet")]
    [ApiController]
    public class DotNetMetricsController : ControllerBase
    {
        private readonly ILogger<DotNetMetricsController> _logger;
        private IRepository<DotNetMetric> _repository;
        private DateTime _UNIX = new DateTime(1970, 01, 01);

        public DotNetMetricsController(ILogger<DotNetMetricsController> logger, IRepository<DotNetMetric> repository)
        {
            _repository = repository;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в DotNetMetricsController");
        }

        [HttpGet("errors-count/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent([FromRoute] DateTime fromTime, [FromRoute] DateTime toTime)
        {
            _logger.LogInformation($"Входные данные {fromTime} {toTime}");

            var config = new MapperConfiguration(cfg => cfg.CreateMap<DotNetMetric, DotNetMetricDto>());
            var m = config.CreateMapper();

            var metrics = _repository.GetFromTo(fromTime, toTime);

            var response = new AllDotNetMetricsResponse()
            {
                Metrics = new List<DotNetMetricDto>()
            };

            foreach (var metric in metrics)
            {
                response.Metrics.Add(m.Map<DotNetMetricDto>(metric));
            }

            return Ok(response);
        }
    }
}
