using AutoMapper;
using Dom = Function.Blending.Opt.Domain.Entities;
using Res = Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Mappings
{
  public sealed class StartLogEjecucionProfiles : Profile
  {
    public StartLogEjecucionProfiles()
    {
      // Dominio → Response (slice Start). Estado se toma del VO 'Estado' (no del Guid).
      CreateMap<Dom.LogEjecucion, Res.StartLogEjecucionResponse>()
        .ForMember(d => d.Estado, cfg => cfg.MapFrom(s => s.Estado));
    }
  }
}
