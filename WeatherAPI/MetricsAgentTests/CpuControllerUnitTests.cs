using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using System;
using Xunit;
using MetricsInfrastucture.Enums;
using Moq;
using MetricsAgent;
using Microsoft.Extensions.Logging;

namespace MetricsAgentTests
{
    public class CpuControllerUnitTests
    {
        private CpuMetricsController controller;
        private Mock<IRepository<CpuMetric>> mock;
        private readonly ILogger<CpuMetricsController> _logger;

        public CpuControllerUnitTests()
        {
            mock = new Mock<IRepository<CpuMetric>>();
            controller = new CpuMetricsController(_logger, mock.Object);
        }

        [Fact]
        public void GetFromTimeToTime()
        {
            mock.Setup(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();

            DateTime d1 = new DateTime(2011, 10, 10);
            DateTime d2 = new DateTime(2011, 11, 11);
            DateTimeOffset fromTime = new DateTimeOffset(d1);
            DateTimeOffset toTime = new DateTimeOffset(d2);

            var result = controller.GetMetricsFromAgent(fromTime, toTime);

            mock.Verify(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());


        }

        //[Fact]
        //public void GetMetricsFromAgent_ReturnsOk()
        //{
        //    var fromTime = TimeSpan.FromSeconds(0);
        //    var toTime = TimeSpan.FromSeconds(100);

        //    var result = _controller.GetMetricsFromAgent(fromTime, toTime);

        //    _ = Assert.IsAssignableFrom<IActionResult>(result);
        //}

        //[Fact]
        //public void GetMetricsByPercentileFromAgent_ReturnsOk()
        //{
        //    var fromTime = TimeSpan.FromSeconds(0);
        //    var toTime = TimeSpan.FromSeconds(100);
        //    var percentile = Percentile.P99;

        //    var result = _controller.GetMetricsByPercentileFromAgent(fromTime, toTime, percentile);

        //    _ = Assert.IsAssignableFrom<IActionResult>(result);
        //}
    }
}
