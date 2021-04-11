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
            CreateMap<AgentInfo, AgentDto>();
        }
    }
}
