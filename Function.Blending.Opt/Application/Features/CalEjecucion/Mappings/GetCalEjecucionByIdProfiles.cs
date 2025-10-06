using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;

// Aliases
using E =  Function.Blending.Opt.Domain.Entities;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class GetCalEjecucionByIdProfiles : Profile
{
  public GetCalEjecucionByIdProfiles()
  {
    // OJO: NO volver a definir EstadoRef -> EstadoResponse aquí.
    CreateMap<E.CalEjecucion, CalEjecucionResponse>()
      .ForMember(d => d.Estado, cfg => cfg.MapFrom(s => s.Estado))
      .ForMember(d => d.Filtro, cfg => cfg.MapFrom(s => s.Filtro))
      .ForMember(d => d.Parametros, cfg => cfg.MapFrom(s => s.Parametros));
  }
}
