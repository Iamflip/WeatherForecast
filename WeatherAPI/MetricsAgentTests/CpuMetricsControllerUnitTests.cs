using MetricsAgent.Controllers;
using MetricsAgent.DAL;
using System;
using Xunit;
using MetricsInfrastucture.Enums;
using Moq;
using MetricsAgent;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MetricsInfrastucture.Interfaces;

namespace MetricsAgentTests
{
    public class CpuMetricsControllerUnitTests
    {
        private CpuMetricsController _controller;
        private Mock<IRepository<CpuMetric>> _mock;
        private Mock<ILogger<CpuMetricsController>> _logger;
        private Mock<IMapper> _mapper;

        public CpuMetricsControllerUnitTests()
        {
            _mock = new Mock<IRepository<CpuMetric>>();
            _logger = new Mock<ILogger<CpuMetricsController>>();
            _mapper = new Mock<IMapper>();
            _controller = new CpuMetricsController(_logger.Object, _mock.Object, _mapper.Object);
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

        [Fact]
        public void GetFromTimeToTimePercentile()
        {
            _mock.Setup(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();

            DateTimeOffset fromTime = new DateTime(2011, 10, 10);
            DateTimeOffset toTime = new DateTime(2011, 11, 11);
            Percentile percentile = Percentile.P99;

            var result = _controller.GetMetricsByPercentileFromAgent(fromTime, toTime, percentile);

            _mock.Verify(repository => repository.GetFromTo(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
        }
    }
}
