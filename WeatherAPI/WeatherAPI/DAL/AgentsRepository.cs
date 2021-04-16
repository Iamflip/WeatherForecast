using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MetricsInfrastucture.Interfaces;

namespace MetricsManager.DAL
{
    public class AgentsRepository : IAgentsRepository<AgentInfo>
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public void Create(AgentInfo item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO agents(agenturl) VALUES(@agenturl)",
                    new
                    {
                        agenturl = item.AgentAddress
                    });
            }
        }

        public IList<AgentInfo> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<AgentInfo>("SELECT * FROM agents").ToList();
            }
        }

        public AgentInfo GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var result = connection.QuerySingle<AgentInfo>("SELECT * FROM agents ORDER BY id DESC LIMIT 1");

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new AgentInfo
                    {
                        AgentId = 0,
                        AgentAddress = @"http://localhost:5000"
                    };
                }
            }
        }
    }
}
