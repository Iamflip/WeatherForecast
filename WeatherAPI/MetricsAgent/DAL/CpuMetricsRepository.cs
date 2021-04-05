using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent
{
    public class CpuMetricsRepository : IRepository<CpuMetric> 
    {
        private SQLiteConnection _connection;

        public CpuMetricsRepository(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public void Create(CpuMetric item)
        {
            // создаем команду
            using var cmd = new SQLiteCommand(_connection);
            // прописываем в команду SQL запрос на вставку данных
            cmd.CommandText = "INSERT INTO cpumetrics(value, time) VALUES(@value, @time)";

            // добавляем параметры в запрос из нашего объекта
            cmd.Parameters.AddWithValue("@value", item.Value);

            // в таблице будем хранить время в секундах, потому преобразуем перед записью в секунды
            // через свойство
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            // подготовка команды к выполнению
            cmd.Prepare();

            // выполнение команды
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var cmd = new SQLiteCommand(_connection);
            // прописываем в команду SQL запрос на удаление данных
            cmd.CommandText = "DELETE FROM cpumetrics WHERE id=@id";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public IList<CpuMetric> GetAll()
        {
            using var cmd = new SQLiteCommand(_connection);

            // прописываем в команду SQL запрос на получение всех данных из таблицы
            cmd.CommandText = "SELECT * FROM cpumetrics";

            var returnList = new List<CpuMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // пока есть что читать -- читаем
                while (reader.Read())
                {
                    // добавляем объект в список возврата
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        // налету преобразуем прочитанные секунды в метку времени
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }

            return returnList;
        }

        public CpuMetric GetById(int id)
        {
            using var cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "SELECT * FROM cpumetrics WHERE id=@id";

            cmd.Parameters.AddWithValue("@id", id);

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                // если удалось что то прочитать
                if (reader.Read())
                {
                    // возвращаем прочитанное
                    return new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    };
                }
                else
                {
                    // не нашлось запись по идентификатору, не делаем ничего
                    return null;
                }
            }
        }

        public IList<CpuMetric> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using var cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "SELECT * FROM cpumetrics WHERE time >= @fromtime AND time <= @totime";
            cmd.Parameters.AddWithValue("@fromtime", fromTime.ToUnixTimeSeconds());
            cmd.Parameters.AddWithValue("@totile", toTime.ToUnixTimeSeconds());

            var returnList = new List<CpuMetric>();

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    returnList.Add(new CpuMetric
                    {
                        Id = reader.GetInt32(0),
                        Value = reader.GetInt32(1),
                        Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                    });
                }
            }
            return returnList;
        }

        public CpuMetric GetLast()
        {
            using var cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "SELECT * FROM cpumetrics ORDER BY id DESC LIMIT 1;";

            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                return new CpuMetric
                {
                    Id = reader.GetInt32(0),
                    Value = reader.GetInt32(1),
                    Time = TimeSpan.FromSeconds(reader.GetInt32(2))
                };
            }
        }

        public void Update(CpuMetric item)
        {
            using var cmd = new SQLiteCommand(_connection);
            // прописываем в команду SQL запрос на обновление данных
            cmd.CommandText = "UPDATE cpumetrics SET value = @value, time = @time WHERE id=@id;";
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@value", item.Value);
            cmd.Parameters.AddWithValue("@time", item.Time.TotalSeconds);
            cmd.Prepare();

            cmd.ExecuteNonQuery();
        }
    }
}
