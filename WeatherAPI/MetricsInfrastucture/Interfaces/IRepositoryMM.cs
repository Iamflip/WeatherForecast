using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsInfrastucture.Interfaces
{
    public interface IRepositoryMM<T> where T : class
    {
        IList<T> GetFromTo(DateTimeOffset fromTime, DateTimeOffset toTime);

        void Create(T item);

        T GetLast();

        IList<T> GetFromToByAgent(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime);

        T GetLastFromAgent(int agentId);
    }
}
