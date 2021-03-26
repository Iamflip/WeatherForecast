using Microsoft.AspNetCore.Mvc;
using System;
using MetricsManager.Controllers;
using MetricsInfrastucture.Enums;
using Xunit;
using Microsoft.Extensions.Logging;

namespace MetricsManagerTests
{
    public class DotNetControllerUnitTests
    {
        private DotNetMetricsController _controller;
        private readonly ILogger<DotNetMetricsController> _logger;

        public DotNetControllerUnitTests()
        {
            _controller = new DotNetMetricsController(_logger);
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
