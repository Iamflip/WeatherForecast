using AutoMapper;
using MetricsAgent.Metric;
using MetricsAgent.Responses;
using System;

namespace MetricsAgent
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CpuMetric, CpuMetricDto>()
                .ForMember(dto => dto.Time, opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<DotNetMetric, DotNetMetricDto>()
                .ForMember(dto => dto.Time, opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<NetworkMetric, NetworkMetricDto>()
                .ForMember(dto => dto.Time, opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<HddMetric, HddMetricDto>()
                .ForMember(dto => dto.Time, opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
            CreateMap<RamMetric, RamMetricDto>()
                .ForMember(dto => dto.Time, opt => opt.MapFrom(c => DateTimeOffset.FromUnixTimeSeconds((long)c.Time.TotalSeconds)));
        }
    }
}
