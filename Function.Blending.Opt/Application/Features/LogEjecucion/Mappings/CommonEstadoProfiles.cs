using AutoMapper;
using Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;
using Dom = Function.Blending.Opt.Domain.ValueObjects;
using Res = Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Mappings
{
  /// <summary>Mapeo único de EstadoRef -> EstadoResponse para reutilizar en Start/GetById.</summary>
  public sealed class CommonEstadoProfiles : Profile
  {
    public CommonEstadoProfiles()
    {
      CreateMap<EstadoLogisticaSnapshot, Res.EstadoResponse>()
        .ForMember(d => d.Id,     m => m.MapFrom(s => s.Id))
        .ForMember(d => d.Nombre, m => m.MapFrom(s => s.Nombre))
        .ForMember(d => d.Color,  m => m.MapFrom(s => s.Color));
    }
  }
}
