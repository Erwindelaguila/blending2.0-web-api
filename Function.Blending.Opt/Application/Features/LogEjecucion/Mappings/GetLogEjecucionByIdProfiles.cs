using AutoMapper;
using Dom = Function.Blending.Opt.Domain.Entities;
using Res = Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Mappings
{
  public sealed class GetLogEjecucionByIdProfiles : Profile
  {
    public GetLogEjecucionByIdProfiles()
    {
      // Asegura el mapeo del VO Estado -> DTO EstadoResponse (igual que Calidad)
      CreateMap<Dom.LogEjecucion, Res.LogEjecucionResponse>()
        .ForMember(d => d.Oferta, cfg => cfg.MapFrom(s => s.Oferta))
        .ForMember(d => d.Estado, cfg => cfg.MapFrom(s => s.Estado));
    }
  }
}
