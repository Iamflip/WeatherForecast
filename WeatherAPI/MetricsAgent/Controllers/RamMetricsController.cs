using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Management;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]
    public class RamMetricsController : ControllerBase
    {
        private readonly ILogger<RamMetricsController> _logger;

        public RamMetricsController (ILogger<RamMetricsController> logger)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog встроен в RamMetricsController");
        }

        [HttpGet("available")]
        public IActionResult GetMetricsFromAgent()
        {
            ManagementObjectSearcher ramMonitor =    //запрос к WMI для получения памяти ПК
                        new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem");

            ulong freeRamMb = 0;

            foreach (ManagementObject objram in ramMonitor.Get())
            {
                freeRamMb = (Convert.ToUInt64(objram["FreePhysicalMemory"]) / 1024);
            }

            return Ok(freeRamMb);
        }
    }
}
