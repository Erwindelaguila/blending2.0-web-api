using AutoMapper;
using Function.Blending.Opt.Infrastructure.Persistence.Models;

// Aliases dominio
using CalEjecucionDomain = Function.Blending.Opt.Domain.Entities.CalEjecucion;
using CalEjecucionHistoryItemRm = Function.Blending.Opt.Domain.ReadModels.CalEjecucionHistoryItemRm;
// IDs tipados
using V = Function.Blending.Opt.Domain.ValueObjects.Ids;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class CalEjecucionProfiles : Profile
{
  public CalEjecucionProfiles()
  {
    // EF -> Dominio (detalle)
    CreateMap<CalEjecucion, CalEjecucionDomain>()
      .ForMember(d => d.Estado, cfg => cfg.Ignore())
      .ForMember(d => d.EstadoNombre, cfg => cfg.Ignore())
      .ForMember(d => d.Id, o => o.MapFrom(s => new V.EjecucionId(s.Id)))
      .ForMember(d => d.PlantaId, o => o.MapFrom(s => new V.PlantaId(s.PlantaId)))
      .ForMember(d => d.EstadoId, o => o.MapFrom(s => new V.EstadoId(s.EstadoId)))
      .ForMember(d => d.Filtro, o => o.MapFrom(s => s.CalInpFiltro))
      .ForMember(d => d.Parametros, o => o.MapFrom(s => s.CalInpParametro))
      .ForMember(d => d.Resumenes, o => o.MapFrom(s => s.CalOutResumen))
      .ForMember(d => d.Detalles, o => o.MapFrom(s => s.CalOutDetalle));

    // EF -> ReadModel (history)
    CreateMap<CalEjecucion, CalEjecucionHistoryItemRm>()
      .ForMember(d => d.EstadoNombre, cfg => cfg.Ignore());
  }
}
