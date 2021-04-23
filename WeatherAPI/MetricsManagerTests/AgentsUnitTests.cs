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
    public class AgentsUnitTests
    {
        private AgentsController _controller;
        private readonly Mock<ILogger<AgentsController>> _logger;
        private Mock<IAgentsRepository<AgentInfo>> _mock;
        private Mock<IMapper> _mapper;
        public AgentsUnitTests()
        {
            _logger = new Mock<ILogger<AgentsController>>();
            _mock = new Mock<IAgentsRepository<AgentInfo>>();
            _mapper = new Mock<IMapper>();
            _controller = new AgentsController(_logger.Object, _mock.Object, _mapper.Object);
        }

        [Fact]
        public void GetAgents()
        {
            _mock.Setup(repository => repository.GetAll()).Verifiable();

            var result = _controller.ReadRegisteredAgents();

            _mock.Verify(repository => repository.GetAll(), Times.AtMostOnce());
        }
    }
}
