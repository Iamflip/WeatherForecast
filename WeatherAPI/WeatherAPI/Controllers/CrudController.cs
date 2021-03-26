using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        private readonly ValuesHolder _holder;
        private readonly ILogger<CrudController> _logger;

        public CrudController(ValuesHolder holder, ILogger<CrudController> logger)
        {
            _holder = holder;
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в CrudController");
        }

        [HttpPost("create")]
        public IActionResult Create([FromQuery] DateTime date, [FromQuery] int temperatureC)
        {
            _logger.LogInformation($"Входные данные: {date} {temperatureC}");

            WeatherForecast weatherForecast = new WeatherForecast(date, temperatureC);
            _holder.Values.Add(weatherForecast);
            return Ok();
        }

        [HttpPut("update")]
        public IActionResult Update([FromQuery] DateTime date, [FromQuery] int newTemperatureC)
        {
            _logger.LogInformation($"Входные данные: {date} {newTemperatureC}");

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
            _logger.LogInformation($"Входные данные: {firstDate} {lastDate}");

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
            _logger.LogInformation($"Входные данные: {firstDate} {lastDate}");

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
