using AutoMapper;
using MetricsInfrastucture.Interfaces;
using MetricsManager.Models;
using MetricsManager.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly ILogger<AgentsController> _logger;
        private IAgentsRepository<AgentInfo> _agent;
        private IMapper _mapper;

        public AgentsController(ILogger<AgentsController> logger, IAgentsRepository<AgentInfo> agent, IMapper mapper)
        {
            _mapper = mapper;
            _agent = agent;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в AgentsController");
        }

        [HttpPost("register")]
        public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
        {
            _logger.LogInformation($"Входные данные: {agentInfo}");
            return Ok();
        }

        [HttpPut("enable/{agentId}")]
        public IActionResult EnableAgentById([FromRoute] int agentId)
        {
            _logger.LogInformation($"Входные данные: {agentId}");
            return Ok();
        }

        [HttpPut("disable/{agentId}")]
        public IActionResult DisableAgentById([FromRoute] int agentId)
        {
            _logger.LogInformation($"Входные данные: {agentId}");
            return Ok();
        }

        [HttpGet("read")]
        public IActionResult ReadRegisteredAgents()
        {
            IList<AgentInfo> agents = _agent.GetAll();

            if (agents == null)
            {
                return Ok();
            }

            var response = new AllAgentsResponce()
            {
                Agents = new List<AgentDto>()
            };

            foreach (var agent in agents)
            {
                response.Agents.Add(_mapper.Map<AgentDto>(agent));
            }

            return Ok(response);
        }
    }
}
