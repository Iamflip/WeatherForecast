using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.Metric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class NetworkMetricsControllerUnitTests
    {
        private NetworkMetricsController _controller;
        private readonly Mock<ILogger<NetworkMetricsController>> _logger;
        private Mock<IRepository<NetworkMetric>> _mock;

        public NetworkMetricsControllerUnitTests()
        {
            _mock = new Mock<IRepository<NetworkMetric>>();
            _logger = new Mock<ILogger<NetworkMetricsController>>();
            _controller = new NetworkMetricsController(_logger.Object, _mock.Object);
        }

        [Fact]
        public void GetFromTimeToTime()
        {
            _mock.Setup(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();

            DateTime d1 = new DateTime(2011, 10, 10);
            DateTime d2 = new DateTime(2011, 11, 11);
            DateTimeOffset fromTime = new DateTimeOffset(d1);
            DateTimeOffset toTime = new DateTimeOffset(d2);

            var result = _controller.GetMetricsFromAgent(fromTime, toTime);

            _mock.Verify(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
        }
    }
}
