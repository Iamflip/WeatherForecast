using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetricsInfrastucture.Interfaces
{
    public interface IAgentsRepository<T> where T : class
    {
        void Create(T item);

        IList<T> GetAll();

        T GetLast();
    }
}
