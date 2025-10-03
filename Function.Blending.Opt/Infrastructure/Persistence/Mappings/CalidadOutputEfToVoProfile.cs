using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class CalidadOutputEfToVoProfile : Profile
{
  public CalidadOutputEfToVoProfile()
  {
    // Resumen
    CreateMap<Ef.CalOutResumen, VO.CalOutResumen>()
      .ForCtorParam(nameof(VO.CalOutResumen.Parametros), o => o.MapFrom(s => s.CalOutResParametro));

    CreateMap<Ef.CalOutResParametro, VO.CalOutResParametro>();


    // Detalle
    CreateMap<Ef.CalOutDetalle, VO.CalOutDetalle>()
      .ForCtorParam(nameof(VO.CalOutDetalle.Parametros), o => o.MapFrom(s => s.CalOutDetParametro))
      .ForCtorParam(nameof(VO.CalOutDetalle.Otros), o => o.MapFrom(s => s.CalOutDetOtros));

    CreateMap<Ef.CalOutDetParametro, VO.CalOutDetParametro>();
    CreateMap<Ef.CalOutDetOtros, VO.CalOutDetOtros>();
  }
}
