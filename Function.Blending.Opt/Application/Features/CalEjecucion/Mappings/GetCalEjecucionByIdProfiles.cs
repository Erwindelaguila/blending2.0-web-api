using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;

// Aliases
using CalEjecucionDomain = Function.Blending.Opt.Domain.Entities.CalEjecucion;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class GetCalEjecucionByIdProfiles : Profile
{
  public GetCalEjecucionByIdProfiles()
  {
    // OJO: NO volver a definir EstadoRef -> EstadoResponse aquí.
    CreateMap<CalEjecucionDomain, CalEjecucionResponse>()
      .ForMember(d => d.Estado, cfg => cfg.MapFrom(s => s.Estado));
  }
}
