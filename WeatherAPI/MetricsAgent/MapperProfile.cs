using AutoMapper;
using MetricsAgent.Metric;
using MetricsAgent.Responses;
using System;

namespace MetricsAgent
{
    public class MapperProfile : Profile
    {
        private DateTimeOffset _UNIX = new DateTime(1970, 01, 01);
        public MapperProfile()
        {
            CreateMap<CpuMetric, CpuMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<DotNetMetric, DotNetMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<NetworkMetric, NetworkMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<HddMetric, HddMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<RamMetric, RamMetricDto>()
                .ForMember("Time", opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
        }
    }
}
