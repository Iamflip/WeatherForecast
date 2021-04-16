﻿using System;
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
    public class NetworkMetricsRepository : IRepositoryMM<NetworkMetric>
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public NetworkMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(NetworkMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO networkmetrics(value, time, agentid) VALUES(@value, @time, @agentid)",
                    new
                    {
                        value = item.Value,
                        time = item.Time.TotalSeconds,
                        agentid = item.AgentId
                    });
            }
        }

        public IList<NetworkMetric> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<NetworkMetric>("SELECT * FROM networkmetrics WHERE time >= @fromtime AND time <= @totime",
                    new
                    {
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public IList<NetworkMetric> GetFromToByAgent(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<NetworkMetric>("SELECT * FROM networkmetrics WHERE time >= @fromtime AND time <= @totime AND agentid = @agentId",
                    new
                    {
                        agentid = agentId,
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public NetworkMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var result = connection.QuerySingle<NetworkMetric>("SELECT * FROM networkmetrics ORDER BY id DESC LIMIT 1");

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new NetworkMetric
                    {
                        AgentId = 0,
                        Id = 0,
                        Value = 0,
                        Time = TimeSpan.FromSeconds(0)
                    };
                }
            }
        }

        public NetworkMetric GetLastFromAgent(int agentId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var result = connection.QuerySingle<NetworkMetric>("SELECT * FROM cpumetrics ORDER BY id DESC LIMIT 1 WHERE agentid = @agentid",
                    new
                    {
                        agentid = agentId
                    });

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new NetworkMetric
                    {
                        AgentId = 0,
                        Id = 0,
                        Value = 0,
                        Time = TimeSpan.FromSeconds(0)
                    };
                }
            }
        }
    }
}
