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
            CreateMap<CpuMetricDto, CpuMetric>()
                .ForMember(dbModel => dbModel.Time, _ => _.MapFrom((src, dst) => src.Time.ToUnixTimeSeconds()));
            CreateMap<DotNetMetricDto, DotNetMetric>()
                .ForMember(dbModel => dbModel.Time, _ => _.MapFrom((src, dst) => src.Time.ToUnixTimeSeconds()));
            CreateMap<NetworkMetricDto, NetworkMetric>()
                .ForMember(dbModel => dbModel.Time, _ => _.MapFrom((src, dst) => src.Time.ToUnixTimeSeconds()));
            CreateMap<HddMetricDto, HddMetric>()
                .ForMember(dbModel => dbModel.Time, _ => _.MapFrom((src, dst) => src.Time.ToUnixTimeSeconds()));
            CreateMap<RamMetricDto, RamMetricDto>()
                .ForMember(dbModel => dbModel.Time, _ => _.MapFrom((src, dst) => src.Time.ToUnixTimeSeconds()));
        }
    }
}
