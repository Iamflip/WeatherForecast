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
    public class DotNetMetricsRepository : IRepositoryMM<DotNetMetric>
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public DotNetMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }
        public void Create(DotNetMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO dotnetmetrics(value, time, agentid) VALUES(@value, @time, @agentid)",
                    new
                    {
                        value = item.Value,
                        time = item.Time.TotalSeconds,
                        agentid = item.AgentId
                    });
            }
        }

        public IList<DotNetMetric> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<DotNetMetric>("SELECT * FROM dotnetmetrics WHERE time >= @fromtime AND time <= @totime",
                    new
                    {
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public IList<DotNetMetric> GetFromToByAgent(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<DotNetMetric>("SELECT * FROM dotnetmetrics WHERE time >= @fromtime AND time <= @totime AND agentid = @agentId",
                    new
                    {
                        agentid = agentId,
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public DotNetMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    return connection.QuerySingle<DotNetMetric>("SELECT * FROM dotnetmetrics ORDER BY id DESC LIMIT 1");
                }
                catch
                {
                    return null;
                }
            }
        }

        public DotNetMetric GetLastFromAgent(int agentId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                try
                {
                    return connection.QuerySingle<DotNetMetric>("SELECT * FROM cpumetrics ORDER BY id DESC LIMIT 1 WHERE agentid = @agentid",
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
