using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Management;
using MetricsAgent.Metric;
using MetricsAgent.Responses;
using AutoMapper;

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
    }
}
