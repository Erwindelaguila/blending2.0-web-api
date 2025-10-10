using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;

// Aliases
using CalEjecucionDomain = Function.Blending.Opt.Domain.Entities.CalEjecucion;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class StartCalEjecucionProfiles : Profile
{
  public StartCalEjecucionProfiles()
  {
    // OJO: NO volver a definir EstadoRef -> EstadoResponse aquí.
    CreateMap<CalEjecucionDomain, StartCalEjecucionResponse>()
      .ForMember(d => d.Estado, cfg => cfg.MapFrom(s => s.Estado));
  }
}
