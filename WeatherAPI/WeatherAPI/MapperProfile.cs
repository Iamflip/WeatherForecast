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
            CreateMap<AgentInfo, AgentDto>();
        }
    }
}
