using MetricsManagerClient.Request;
using MetricsManagerClient.Responces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MetricsManagerClient.Client
{
    class MetricsClient : IMetricsClient
    {
        private HttpClient _httpClient;
        public MetricsClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public AllCpuMetricsApiResponse GetFromToCpuMetrics(GetAllCpuMetricsApiRequest request)
        {
            var fromParameter = request.FromTime.ToString("O");
            var toParameter = request.ToTime.ToString("O");

            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5000/api/metrics/cpu/cluster/from/{fromParameter}/to/{toParameter}");

            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                var responseStream = response.Content.ReadAsStreamAsync().Result;

                var streamReader = new StreamReader(responseStream);

                return JsonConvert.DeserializeObject<AllCpuMetricsApiResponse>(streamReader.ReadToEnd());
            }
            catch
            {

            }
            return null;
        }
    }
}
