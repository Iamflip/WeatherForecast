using Dapper;
using MetricsAgent.DAL;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using MetricsInfrastucture.Interfaces;
using MetricsInfrastucture.Handlers;

namespace MetricsAgent
{
    public class CpuMetricsRepository : IRepository<CpuMetric> 
    {
        private const string ConnectionString = @"Data Source=metrics.db; Version=3;Pooling=True;Max Pool Size=100;";

        public CpuMetricsRepository()
        {
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public void Create(CpuMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("INSERT INTO cpumetrics(value, time) VALUES(@value, @time)",
                    new
                    {
                        value = item.Value,
                        time = item.Time.TotalSeconds
                    });
            }
        }

        public IList<CpuMetric> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<CpuMetric>("SELECT * FROM cpumetrics WHERE time >= @fromtime AND time <= @totime",
                    new
                    {
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public CpuMetric GetLast()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<CpuMetric>("SELECT * FROM cpumetrics ORDER BY id DESC LIMIT 1");
            }
        }
    }
}
