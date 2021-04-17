using System;
using Xunit;
using MetricsManager.Controllers;
using MetricsInfrastucture.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using MetricsManager.Client;
using MetricsManager.DAL;
using MetricsInfrastucture.Interfaces;
using AutoMapper;
using MetricsManager;
using MetricsManager.Metrics;

namespace MetricsManagerTests
{
    public class DotNetControllerUnitTests
    {
        private DotNetMetricsController _controller;
        private readonly Mock<ILogger<DotNetMetricsController>> _logger;
        private Mock<IMetricsAgentClient> _client;
        private Mock<IRepositoryMM<DotNetMetric>> _mock;
        private Mock<IMapper> _mapper;
        private Mock<IAgentsRepository<AgentInfo>> _agent;

        public DotNetControllerUnitTests()
        {
            _logger = new Mock<ILogger<DotNetMetricsController>>();
            _client = new Mock<IMetricsAgentClient>();
            _mock = new Mock<IRepositoryMM<DotNetMetric>>();
            _mapper = new Mock<IMapper>();
            _agent = new Mock<IAgentsRepository<AgentInfo>>();
            _controller = new DotNetMetricsController(_logger.Object, _client.Object, _mock.Object, _mapper.Object, _agent.Object);
        }

        [Fact]
        public void GetMetricsFromAgent()
        {
            _mock.Setup(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();

            int agentId = 1;
            DateTimeOffset fromTime = new DateTime(2011, 10, 10);
            DateTimeOffset toTime = new DateTime(2011, 11, 11);

            var result = _controller.GetMetricsFromAgent(agentId, fromTime, toTime);

            _mock.Verify(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetMetricsByPercentileFromAgent()
        {
            _mock.Setup(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();

            int agentId = 1;
            DateTimeOffset fromTime = new DateTime(2011, 10, 10);
            DateTimeOffset toTime = new DateTime(2011, 11, 11);
            Percentile percentile = Percentile.P99;

            var result = _controller.GetMetricsByPercentileFromAgent(agentId, fromTime, toTime, percentile);

            _mock.Verify(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetMetricsFromAllCluster()
        {
            _mock.Setup(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();
            _agent.Setup(rep => rep.GetAll()).Verifiable();

            DateTimeOffset fromTime = new DateTime(2011, 10, 10);
            DateTimeOffset toTime = new DateTime(2011, 11, 11);

            var result = _controller.GetMetricsFromAllCluster(fromTime, toTime);

            _mock.Verify(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
            _agent.Verify(rep => rep.GetAll(), Times.AtMostOnce());
        }

        [Fact]
        public void GetMetricsByPercentileFromAllCluster()
        {
            _mock.Setup(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Verifiable();
            _agent.Setup(rep => rep.GetAll()).Verifiable();

            DateTimeOffset fromTime = new DateTime(2011, 10, 10);
            DateTimeOffset toTime = new DateTime(2011, 11, 11);
            Percentile percentile = Percentile.P99;

            var result = _controller.GetMetricsByPercentileFromAllCluster(fromTime, toTime, percentile);

            _mock.Verify(repository => repository.GetFromToByAgent(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
            _agent.Verify(rep => rep.GetAll(), Times.AtMostOnce());
        }
    }
}
