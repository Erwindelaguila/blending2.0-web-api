using AutoMapper;
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models;
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class LogEjecucionOutputVoToEfProfiles : Profile
{
  public LogEjecucionOutputVoToEfProfiles()
  {
    // Dominio → EF (Contenedores)
    CreateMap<VO.LogOutContenedor, EF.LogOutContenedor>()
      .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
      .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
      .ForMember(d => d.CreadoPorId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetUserId()))
      .ForMember(d => d.CreadoEl, o => o.MapFrom(_ => DateTime.UtcNow))
      .ForMember(d => d.Ejecucion, o => o.Ignore())
      .ForMember(d => d.LogOutConDistribucion, o => o.MapFrom(s => s.LogOutConDistribucion))
      .ForMember(d => d.LogOutConComposicion, o => o.MapFrom(s => s.LogOutConComposicion));

    // Dominio → EF (Distribucion)
    CreateMap<VO.LogOutConDistribucion, EF.LogOutConDistribucion>()
      .ForMember(d => d.Contenedor, o => o.Ignore());

    // Dominio → EF (Composicion)
    CreateMap<VO.LogOutConComposicion, EF.LogOutConComposicion>()
      .ForMember(d => d.Contenedor, o => o.Ignore());
  }
}
