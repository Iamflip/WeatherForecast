using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.Metric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class HddControllerUnitTests
    {
        private HddMetricsController _controller;
        private readonly ILogger<HddMetricsController> _logger;
        private IRepository<HddMetric> _repository;

        public HddControllerUnitTests()
        {
            _controller = new HddMetricsController(_logger, _repository);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnsOk()
        {
            var result = _controller.GetMetricsFromAgent();

            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
