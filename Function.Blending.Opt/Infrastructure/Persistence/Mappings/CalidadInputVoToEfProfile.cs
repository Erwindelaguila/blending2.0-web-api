using System;
using AutoMapper;
using DomainVO = Function.Blending.Opt.Domain.ValueObjects;
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;
using Function.Blending.Opt.Infrastructure.Persistence.Mappings;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings
{
  public sealed class CalidadInputVoToEfProfile : Profile
  {
    public CalidadInputVoToEfProfile()
    {
      CreateMap<DomainVO.CalInpFiltro, Ef.CalInpFiltro>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
        .ForMember(d => d.FechaCorte, o => o.MapFrom(s => s.FechaCorteUtc))
        .ForMember(d => d.Ejecucion, o => o.Ignore());

      CreateMap<DomainVO.CalInpParametro, Ef.CalInpParametro>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
        // === cambio aquí: VO -> Guid ===
        .ForMember(d => d.CalidadId, o => o.MapFrom(s => s.CalidadId.Value))
        .ForMember(d => d.ParametroId, o => o.MapFrom(s => s.ParametroId.Value))
        .ForMember(d => d.Calidad, o => o.Ignore())
        .ForMember(d => d.Ejecucion, o => o.Ignore())
        .ForMember(d => d.Parametro, o => o.Ignore());
    }
  }
}
