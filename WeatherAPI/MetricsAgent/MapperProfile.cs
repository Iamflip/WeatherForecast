using AutoMapper;
using MetricsAgent.Metric;
using MetricsAgent.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsAgent
{
    public class MapperProfile : Profile
    {
        private DateTimeOffset _UNIX = new DateTime(1970, 01, 01);
        public MapperProfile()
        {
            CreateMap<CpuMetric, CpuMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => _UNIX.AddSeconds(c.Time.TotalSeconds)));
            CreateMap<DotNetMetric, DotNetMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => _UNIX.AddSeconds(c.Time.TotalSeconds)));
            CreateMap<HddMetric, HddMetricDto>();
            CreateMap<NetworkMetric, NetworkMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => _UNIX.AddSeconds(c.Time.TotalSeconds)));
            CreateMap<RamMetric, RamMetricDto>();
        }
    }
}
