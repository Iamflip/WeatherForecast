using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        private readonly ValuesHolder _holder;

        public CrudController(ValuesHolder holder)
        {
            _holder = holder;
        }

        [HttpPost("create")]
        public IActionResult Create([FromQuery] DateTime date, [FromQuery] int temperatureC)
        {
            WeatherForecast weatherForecast = new WeatherForecast(date, temperatureC);
            _holder.Values.Add(weatherForecast);
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult Update([FromQuery] DateTime date, [FromQuery] int newTemperatureC)
        {
            foreach (var WeatherForecast in _holder.Values)
            {
                if (date == WeatherForecast.Date)
                {
                    WeatherForecast.TemperatureC = newTemperatureC;
                }
            }
            return Ok();
        }

        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] DateTime firstDate, [FromQuery] DateTime lastDate)
        {
            for (int i = _holder.Values.Count - 1; i >= 0; i--)
            {
                if (_holder.Values[i].Date >= firstDate && _holder.Values[i].Date <= lastDate)
                {
                    _holder.Values.RemoveAt(i);
                }
            }
            return Ok();
        }

        [HttpGet("read")]
        public IActionResult Read()
        {
            return Ok(_holder.Values);
        }

        [HttpGet("readfromto")]
        public IActionResult ReadFromTo([FromQuery] DateTime firstDate, [FromQuery] DateTime lastDate)
        {
            List<WeatherForecast> weathers = new List<WeatherForecast>();

            foreach (var WeatherForecast in _holder.Values)
            {
                if (WeatherForecast.Date >= firstDate && WeatherForecast.Date <= lastDate)
                {
                    weathers.Add(WeatherForecast);
                }
            }

            return Ok(weathers);
        }
    }
}
