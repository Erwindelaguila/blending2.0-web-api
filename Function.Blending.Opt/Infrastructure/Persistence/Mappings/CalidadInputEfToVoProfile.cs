using System;
using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings
{
  public sealed class CalidadInputEfToVoProfile : Profile
  {
    public CalidadInputEfToVoProfile()
    {
      CreateMap<Ef.CalInpFiltro, VO.CalInpFiltro>()
        .ForCtorParam(nameof(VO.CalInpFiltro.FechaCorteUtc), opt => opt.MapFrom(src => src.FechaCorte));

      CreateMap<Ef.CalInpParametro, VO.CalInpParametro>()
        .ForCtorParam(nameof(VO.CalInpParametro.CalidadId), o => o.MapFrom(s => new VO.Ids.CalidadId(s.CalidadId)))
        .ForCtorParam(nameof(VO.CalInpParametro.ParametroId), o => o.MapFrom(s => new VO.Ids.ParametroId(s.ParametroId)));
    }
  }
}
