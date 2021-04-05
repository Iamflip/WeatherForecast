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
    public class HddMetricsControllerUnitTests
    {
        private HddMetricsController _controller;
        private readonly Mock<ILogger<HddMetricsController>> _logger;
        private Mock<IRepository<HddMetric>> _mock;

        public HddMetricsControllerUnitTests()
        {
            _mock = new Mock<IRepository<HddMetric>>();
            _logger = new Mock<ILogger<HddMetricsController>>();
            _controller = new HddMetricsController(_logger.Object, _mock.Object);
        }

        [Fact]
        public void GetFromTimeToTime()
        {
            _mock.Setup(repository => repository.GetLast());

            var result = _controller.GetMetricsFromAgent();

            _mock.Verify(repository => repository.GetLast(), Times.AtMostOnce());
        }
    }
}
