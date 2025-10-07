using AutoMapper;
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;
using Dom = Function.Blending.Opt.Domain.Entities;
using Rm = Function.Blending.Opt.Domain.ReadModels;
using V = Function.Blending.Opt.Domain.ValueObjects.Ids;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class LogEjecucionProfiles : Profile
{
  public LogEjecucionProfiles()
  {
    // EF → Dominio (ById)
    CreateMap<Ef.LogEjecucion, Dom.LogEjecucion>()
      .ForMember(d => d.Id, o => o.MapFrom(s => new V.EjecucionId(s.Id)))
      .ForMember(d => d.EstadoId, o => o.MapFrom(s => new V.EstadoId(s.EstadoId)))
      .ForMember(d => d.Estado, cfg => cfg.Ignore())
      .ForMember(d => d.EstadoNombre, cfg => cfg.Ignore())
      .ForMember(d => d.Info, o => o.MapFrom(s => s.LogInpInfo))
      .ForMember(d => d.Filtro, o => o.MapFrom(s => s.LogInpFiltro))
      .ForMember(d => d.Demanda, o => o.MapFrom(s => s.LogInpDemanda))
      .ForMember(d => d.Oferta, o => o.MapFrom(s => s.LogInpOferta))
      .ForMember(d => d.Contenedores, o => o.MapFrom(s => s.LogOutContenedor));

    // EF → ReadModel (history)
    CreateMap<Ef.LogEjecucion, Rm.LogEjecucionHistoryItemRm>()
      .ForMember(d => d.Contrato, o => o.MapFrom(s => s.LogInpInfo != null? s.LogInpInfo.Contrato : null))
      .ForMember(d => d.EstadoNombre, cfg => cfg.Ignore())
      .ForMember(d => d.EstadoColor, cfg => cfg.Ignore());
  }
}
