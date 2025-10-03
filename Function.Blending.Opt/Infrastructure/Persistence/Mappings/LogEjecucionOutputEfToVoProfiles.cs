using AutoMapper;
using EF = Function.Blending.Opt.Infrastructure.Persistence.Models;
using VO = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class LogEjecucionOutputEfToVoProfiles : Profile
{
  public LogEjecucionOutputEfToVoProfiles()
  {
    // EF → Dominio (Contenedores)
    CreateMap<EF.LogOutContenedor, VO.LogOutContenedor>();

    // EF → Dominio (Distribucion)
    CreateMap<EF.LogOutConDistribucion, VO.LogOutConDistribucion>();

    // EF → Dominio (Composicion)
    CreateMap<EF.LogOutConComposicion, VO.LogOutConComposicion>();
  }
}
