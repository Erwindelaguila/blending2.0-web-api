using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using DtoReq = Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class CalidadOutputDtoToVoProfile : Profile
{
  public CalidadOutputDtoToVoProfile()
  {
    // Resumen
    CreateMap<DtoReq.CalOutResumenDto, VO.CalOutResumen>()
      .ForCtorParam(nameof(VO.CalOutResumen.Id), o => o.MapFrom(_ => (Guid?)null))
      .ForCtorParam(nameof(VO.CalOutResumen.EjecucionId), o => o.MapFrom(_ => (Guid?)null));

    CreateMap<DtoReq.CalOutResParametroDto, VO.CalOutResParametro>()
      .ForCtorParam(nameof(VO.CalOutResParametro.Id), o => o.MapFrom(_ => (Guid?)null))
      .ForCtorParam(nameof(VO.CalOutResParametro.ResumenId), o => o.MapFrom(_ => (Guid?)null));

    // Detalle
    CreateMap<DtoReq.CalOutDetalleDto, VO.CalOutDetalle>()
      .ForCtorParam(nameof(VO.CalOutDetalle.Id), o => o.MapFrom(_ => (Guid?)null))
      .ForCtorParam(nameof(VO.CalOutDetalle.EjecucionId), o => o.MapFrom(_ => (Guid?)null));

    CreateMap<DtoReq.CalOutDetParametroDto, VO.CalOutDetParametro>()
      .ForCtorParam(nameof(VO.CalOutDetParametro.Id), o => o.MapFrom(_ => (Guid?)null))
      .ForCtorParam(nameof(VO.CalOutDetParametro.DetalleId), o => o.MapFrom(_ => (Guid?)null));

    CreateMap<DtoReq.CalOutDetOtrosDto, VO.CalOutDetOtros>()
      .ForCtorParam(nameof(VO.CalOutDetOtros.Id), o => o.MapFrom(_ => (Guid?)null))
      .ForCtorParam(nameof(VO.CalOutDetOtros.DetalleId), o => o.MapFrom(_ => (Guid?)null));
  }
}
