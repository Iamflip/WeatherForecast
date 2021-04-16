using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetricsInfrastucture.Interfaces;
using MetricsManager.Metrics;
using MetricsInfrastucture.Handlers;
using Dapper;
using System.Data.SQLite;

namespace MetricsManager.DAL
{
    public class RamMetricsRepository : IRepositoryMM<RamMetric>
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public RamMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(RamMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO rammetrics(value, time, agentid) VALUES(@value, @time, @agentid)",
                    new
                    {
                        value = item.Value,
                        time = item.Time.TotalSeconds,
                        agentid = item.AgentId
                    });
            }
        }

        public IList<RamMetric> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<RamMetric>("SELECT * FROM cpumetric WHERE time >= @fromtime AND time <= @totime",
                    new
                    {
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public IList<RamMetric> GetFromToByAgent(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<RamMetric>("SELECT * FROM rammetrics WHERE time >= @fromtime AND time <= @totime AND agentid = @agentId",
                    new
                    {
                        agentid = agentId,
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public RamMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    return connection.QuerySingle<RamMetric>("SELECT * FROM rammetrics ORDER BY id DESC LIMIT 1");
                }
                catch
                {
                    return null;
                }
            }
        }

        public RamMetric GetLastFromAgent(int agentId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    return connection.QuerySingle<RamMetric>("SELECT * FROM rammetrics ORDER BY id DESC LIMIT 1 WHERE agentid = @agentid",
                        new
                        {
                            agentid = agentId
                        });
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
