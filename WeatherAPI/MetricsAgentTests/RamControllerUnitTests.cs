using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.Metric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class RamControllerUnitTests
    {
        private RamMetricsController _controller;
        private readonly ILogger<RamMetricsController> _logger;
        private IRepository<RamMetric> _repository;

        public RamControllerUnitTests()
        {
            _controller = new RamMetricsController(_logger);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnsOk()
        {
            var result = _controller.GetMetricsFromAgent();

            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
