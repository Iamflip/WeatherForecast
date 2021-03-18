using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI
{
    public class ValuesHolder
    {
        public List<WeatherForecast> Values { get; set; }

        public ValuesHolder()
        {
            Values = new List<WeatherForecast>();
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Values.Count; i++)
            {
                yield return Values[i];
            }
        }
    }
}
