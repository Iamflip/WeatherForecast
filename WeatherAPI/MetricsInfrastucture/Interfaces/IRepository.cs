using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsInfrastucture.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IList<T> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime);

        void Create(T item);

        T GetLast();
    }
}
