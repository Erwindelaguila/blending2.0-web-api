using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

/// <summary>Mapeo único de EstadoRef -> EstadoResponse para reutilizar en GetById/Start.</summary>
public sealed class CommonEstadoProfiles : Profile
{
  public CommonEstadoProfiles()
  {
    CreateMap<EstadoCalidadSnapshot, EstadoResponse>()
      .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
      .ForMember(d => d.Nombre, m => m.MapFrom(s => s.Nombre))
      .ForMember(d => d.Color, m => m.MapFrom(s => s.Color));
  }
}
