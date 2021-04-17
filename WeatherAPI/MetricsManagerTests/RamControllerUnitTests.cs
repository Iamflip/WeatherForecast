using System;
using Xunit;
using MetricsManager.Controllers;
using Microsoft.AspNetCore.Mvc;
using MetricsInfrastucture.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace MetricsManagerTests
{
    public class RamControllerUnitTests
    {
        private RamMetricsController _controller;
        private readonly Mock<ILogger<RamMetricsController>> _logger;

        public RamControllerUnitTests()
        {
            _logger = new Mock<ILogger<RamMetricsController>>();
            _controller = new RamMetricsController(_logger.Object);
        }

        [Fact]
        public void GetMetricsFromAgent_ReturnsOk()
        {
            //Arrange
            var agentId = 1;
            var fromTime = new DateTime(2012, 11, 11);
            var toTime = new DateTime(2013, 11, 11);

            //Act
            var result = _controller.GetMetricsFromAgent(agentId, fromTime, toTime);

            // Assert
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsByPercentileFromAgent_ReturnsOk()
        {
            var agentId = 1;
            var fromTime = new DateTime(2012, 11, 11);
            var toTime = new DateTime(2013, 11, 11);
            var percentile = Percentile.P99;

            var result = _controller.GetMetricsByPercentileFromAgent(agentId, fromTime, toTime, percentile);

            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsFromAllCluster()
        {
            var fromTime = new DateTime(2012, 11, 11);
            var toTime = new DateTime(2013, 11, 11);

            var result = _controller.GetMetricsFromAllCluster(fromTime, toTime);

            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetMetricsByPercentileFromAllCluster()
        {
            var fromTime = new DateTime(2012, 11, 11);
            var toTime = new DateTime(2013, 11, 11);
            var percentile = Percentile.P99;

            var result = _controller.GetMetricsByPercentileFromAllCluster(fromTime, toTime, percentile);

            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
