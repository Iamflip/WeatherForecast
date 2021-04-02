using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using MetricsAgent.Metric;
using Dapper;

namespace MetricsAgent.DAL
{
    public class RamMetricsRepository : IRepository<RamMetric>
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
                connection.Execute("INSERT INTO rammetrics (value, time) VALUES (@value, @time)",
                    new
                    {
                        value = item.Value,
                        time = item.Time.TotalSeconds
                    });
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("DELETE FROM rammetrics WHERE id = @id",
                    new { id = id });
            }
        }

        public IList<RamMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<RamMetric>("SELECT * FROM rammetrics").ToList();
            }
        }

        public RamMetric GetById(int id)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.QuerySingle<RamMetric>("SELECT * FROM rammetrics WHERE id = @id",
                    new { id = id });
            }
        }

        public IList<RamMetric> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                return connection.Query<RamMetric>("SELECT * FROM rammetrics WHERE time >= @fromtime AND time <= @totime",
                    new
                    {
                        fromtime = fromTime.ToUnixTimeSeconds(),
                        totime = toTime.ToUnixTimeSeconds()
                    }).ToList();
            }
        }

        public void Update(RamMetric item)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Execute("UPDATE rammetrics SET value = @value, time = @time WHERE id = @id",
                    new
                    {
                        value = item.Value,
                        time = item.Time.TotalSeconds,
                        id = item.Id
                    });
            }
        }
    }
}
