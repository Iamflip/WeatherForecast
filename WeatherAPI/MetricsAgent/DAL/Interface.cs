using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetAll();

        IList<T> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime);

        T GetById(int id);

        void Create(T item);

        void Update(T item);

        void Delete(int id);
    }
}
