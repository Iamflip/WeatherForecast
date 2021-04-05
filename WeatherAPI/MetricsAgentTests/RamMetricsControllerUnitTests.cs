using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.Metric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;
using Moq;

namespace MetricsAgentTests
{
    public class RamMetricsControllerUnitTests
    {
        private RamMetricsController _controller;
        private readonly Mock<ILogger<RamMetricsController>> _logger;
        private Mock<IRepository<RamMetric>> _mock;

        public RamMetricsControllerUnitTests()
        {
            _mock = new Mock<IRepository<RamMetric>>();
            _logger = new Mock<ILogger<RamMetricsController>>();
            _controller = new RamMetricsController(_logger.Object, _mock.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnsOk()
        {
            var result = _controller.GetMetricsFromAgent();

            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
