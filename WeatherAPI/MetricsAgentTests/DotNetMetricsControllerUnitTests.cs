using AutoMapper;
using MetricsAgent;
using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace MetricsAgentTests
{
    public class DotNetMetricsControllerUnitTests
    {
        private DotNetMetricsController _controller;
        private Mock<ILogger<DotNetMetricsController>> _logger;
        private Mock<IRepository<DotNetMetric>> _mock;
        private Mock<IMapper> _mapper;

        public DotNetMetricsControllerUnitTests()
        {
            _logger = new Mock<ILogger<DotNetMetricsController>>();
            _mock = new Mock<IRepository<DotNetMetric>>();
            _mapper = new Mock<IMapper>();
            _controller = new DotNetMetricsController(_logger.Object, _mock.Object, _mapper.Object);
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
