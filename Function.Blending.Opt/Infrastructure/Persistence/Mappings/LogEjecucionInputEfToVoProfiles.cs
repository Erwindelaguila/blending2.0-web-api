using AutoMapper;
using Function.Blending.Opt.Domain.ValueObjects.Ids;
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models;
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class LogEjecucionInputEfToVoProfiles : Profile
{
  public LogEjecucionInputEfToVoProfiles()
  {
    // EF → Dominio (Info)
    CreateMap<EF.LogInpInfo, VO.LogInpInfo>();

    // EF → Dominio (Demanda)
    CreateMap<EF.LogInpDemanda, VO.LogInpDemanda>()
      .ForMember(d => d.Parametros, o => o.MapFrom(s => s.LogInpDemParametro));

    CreateMap<EF.LogInpDemParametro, VO.LogInpDemParametro>();

    // EF → Dominio (Oferta)
    CreateMap<EF.LogInpOferta, VO.LogInpOferta>()
      .ForMember(d => d.Parametros, o => o.MapFrom(s => s.LogInpOfeParametro))
      .ForMember(d => d.Otros, o => o.MapFrom(s => s.LogInpOfeOtros));

    CreateMap<EF.LogInpOfeParametro, VO.LogInpOfeParametro>();
    CreateMap<EF.LogInpOfeOtros, VO.LogInpOfeOtros>();


    // EF → Dominio (Filtro)
    CreateMap<EF.LogInpFiltro, VO.LogInpFiltro>()
      .ForMember(d => d.Capacidades, o => o.MapFrom(s => s.LogInpFilCapacidad))
      .ForMember(d => d.Divisiones, o => o.MapFrom(s => s.LogInpFilDivision))
      .ForMember(d => d.Emparejamientos, o => o.MapFrom(s => s.LogInpFilEmparejamiento));

    CreateMap<EF.LogInpFilCapacidad, VO.LogInpFilCapacidad>();
    CreateMap<EF.LogInpFilDivision, VO.LogInpFilDivision>();
    CreateMap<EF.LogInpFilEmparejamiento, VO.LogInpFilEmparejamiento>();
  }
}
