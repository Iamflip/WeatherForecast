using MetricsAgent;
using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class DotNetControllerUnitTests
    {
        private DotNetMetricsController _controller;
        private readonly ILogger<DotNetMetricsController> _logger;
        private IRepository<DotNetMetric> _repository;

        public DotNetControllerUnitTests()
        {
            _controller = new DotNetMetricsController(_logger, _repository);
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
