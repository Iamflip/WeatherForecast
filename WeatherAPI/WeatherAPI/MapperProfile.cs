using AutoMapper;
using MetricsManager.Metrics;
using MetricsManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager
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
            CreateMap<AgentInfo, AgentDto>();
        }
    }
}
