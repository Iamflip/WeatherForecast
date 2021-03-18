using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        private readonly ValuesHolder holder;

        public CrudController(ValuesHolder holder)
        {
            this.holder = holder;
        }

        [HttpPost("create")]
        public IActionResult Create(DateTime date, int temperatureC)
        {
            WeatherForecast weatherForecast = new WeatherForecast(date, temperatureC);
            holder.Values.Add(weatherForecast);
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult Update(DateTime date, int newTemperatureC)
        {
            int i = 0;
            foreach (var hold in holder)
            {
                if (date == holder.Values[i].Date)
                {
                    holder.Values[i].TemperatureC = newTemperatureC;
                }
                i++;
            }
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete(DateTime firstDate, DateTime lastDate)
        {
            for (int i = holder.Values.Count - 1; i >= 0; i--)
            {
                if (holder.Values[i].Date >= firstDate && holder.Values[i].Date <= lastDate)
                {
                    holder.Values.RemoveAt(i);
                }
            }
            return Ok();
        }

        [HttpGet("read")]
        public IActionResult Read()
        {
            return Ok(holder.Values);
        }

        [HttpGet("readfromto")]
        public IActionResult ReadFromTo(DateTime firstDate, DateTime lastDate)
        {
            List<WeatherForecast> weathers = new List<WeatherForecast>();

            for (int i = 0; i < holder.Values.Count; i++)
            {
                if (holder.Values[i].Date >= firstDate && holder.Values[i].Date <= lastDate)
                {
                    weathers.Add(holder.Values[i]);
                }
            }

            return Ok(weathers);
        }
    }
}
