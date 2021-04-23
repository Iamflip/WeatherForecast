using AutoMapper;
using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.Metric;
using MetricsInfrastucture.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class RamMetricsControllerUnitTests
    {
        private RamMetricsController _controller;
        private readonly Mock<ILogger<RamMetricsController>> _logger;
        private Mock<IRepository<RamMetric>> _mock;
        private Mock<IMapper> _mapper;

        public RamMetricsControllerUnitTests()
        {
            _mock = new Mock<IRepository<RamMetric>>();
            _logger = new Mock<ILogger<RamMetricsController>>();
            _mapper = new Mock<IMapper>();
            _controller = new RamMetricsController(_logger.Object, _mock.Object, _mapper.Object);
        }

        [Fact]
        public void GetLastAvailable()
        {
            _mock.Setup(repository => repository.GetLast());

            var result = _controller.GetMetricsFromAgent();

            _mock.Verify(repository => repository.GetLast(), Times.AtMostOnce());
        }

        [Fact]
        public void GetFromTimeToTime()
        {
            _mock.Setup(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();

            DateTimeOffset fromTime = new DateTime(2011, 10, 10);
            DateTimeOffset toTime = new DateTime(2011, 11, 11);

            var result = _controller.GetMetricsFromAgent(fromTime, toTime);

            _mock.Verify(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
        }
    }
}
