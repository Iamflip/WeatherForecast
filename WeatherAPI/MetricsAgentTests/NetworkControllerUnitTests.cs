using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.Metric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class NetworkControllerUnitTests
    {
        private NetworkMetricsController _controller;
        private readonly ILogger<NetworkMetricsController> _logger;
        private IRepository<NetworkMetric> _repository;

        public NetworkControllerUnitTests()
        {
            _controller = new NetworkMetricsController(_logger, _repository);
        }

        //[Fact]
        //public void GetMetricsFromAgent_ReturnsOk()
        //{
        //    var fromTime = new DateTime(2012, 11, 11);
        //    var toTime = new DateTime(2013, 11, 11);

        //    var result = _controller.GetMetricsFromAgent(fromTime, toTime);

        //    _ = Assert.IsAssignableFrom<IActionResult>(result);
        //}
    }
}
