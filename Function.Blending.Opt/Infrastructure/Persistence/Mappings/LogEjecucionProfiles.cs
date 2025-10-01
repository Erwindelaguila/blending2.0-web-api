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
    // EF → Dominio (detalle)
    CreateMap<Ef.LogEjecucion, Dom.LogEjecucion>()
      .ForMember(d => d.Estado, cfg => cfg.Ignore())
      .ForMember(d => d.EstadoNombre, cfg => cfg.Ignore())
      .ForMember(d => d.Id, o => o.MapFrom(s => new V.EjecucionId(s.Id)))
      .ForMember(d => d.EstadoId, o => o.MapFrom(s => new V.EstadoId(s.EstadoId)));

    // EF → ReadModel (history)
    CreateMap<Ef.LogEjecucion, Rm.LogEjecucionHistoryItemRm>()
      .ForMember(d => d.Contrato, cfg => cfg.Ignore())
      .ForMember(d => d.EstadoNombre, cfg => cfg.Ignore());
  }
}
