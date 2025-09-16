using System;
using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;
using Function.Blending.Opt.Infrastructure.Persistence.Mappings;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class CalidadOutputVoToEfProfile : Profile
{
  public CalidadOutputVoToEfProfile()
  {
    // =======================
    // VO → EF (Resumen)
    // =======================
    CreateMap<VO.CalOutResumen, Ef.CalOutResumen>()
      .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
      .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
      //.ForMember(d => d.NuevaFechaFabricacion, o => o.MapFrom(s => s.NuevaFechaFabricacion))
      .ForMember(d => d.Aceptado, o => o.MapFrom(s => s.Aceptado ?? false))
      .ForMember(d => d.CreadoPorId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetUserId()))
      .ForMember(d => d.CreadoEl, o => o.MapFrom(_ => DateTime.UtcNow))
      // Ignorar navegaciones y campos de modificación (se insertan una única vez)
      .ForMember(d => d.Ejecucion, o => o.Ignore())
      .ForMember(d => d.CalOutResParametro, o => o.Ignore())
      .ForMember(d => d.ModificadoPorId, o => o.Ignore())
      .ForMember(d => d.ModificadoEl, o => o.Ignore());

    // VO → EF (Resumen.Parametros)
    CreateMap<VO.CalOutResParametro, Ef.CalOutResParametro>()
      .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
      .ForMember(d => d.ResumenId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetParentId()))
      .ForMember(d => d.Resumen, o => o.Ignore());

    // =======================
    // VO → EF (Detalle)
    // =======================
    CreateMap<VO.CalOutDetalle, Ef.CalOutDetalle>()
      .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
      .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
      .ForMember(d => d.Aceptado, o => o.MapFrom(s => s.Aceptado ?? false))
      .ForMember(d => d.CreadoPorId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetUserId()))
      .ForMember(d => d.CreadoEl, o => o.MapFrom(_ => DateTime.UtcNow))
      // Ignorar navegaciones y campos de modificación (se insertan una única vez)
      .ForMember(d => d.Ejecucion, o => o.Ignore())
      .ForMember(d => d.CalOutDetParametro, o => o.Ignore())
      .ForMember(d => d.CalOutDetOtros, o => o.Ignore())
      .ForMember(d => d.ModificadoPorId, o => o.Ignore())
      .ForMember(d => d.ModificadoEl, o => o.Ignore());

    // VO → EF (Detalle.Parametros)
    CreateMap<VO.CalOutDetParametro, Ef.CalOutDetParametro>()
      .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
      .ForMember(d => d.DetalleId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetParentId()))
      .ForMember(d => d.Detalle, o => o.Ignore());

    // VO → EF (Detalle.Otros)
    CreateMap<VO.CalOutDetOtros, Ef.CalOutDetOtros>()
      .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
      .ForMember(d => d.DetalleId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetParentId()))
      .ForMember(d => d.Detalle, o => o.Ignore());


    // =======================
    // VO → EF (Resumen Response)
    // =======================
    CreateMap<Ef.CalOutResumen, VO.CalOutResumen>();
  }
}
