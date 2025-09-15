using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using DtoReq = Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;
using DtoRes = Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Response.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class CalidadOutputDtoToVoProfile : Profile
{
  public CalidadOutputDtoToVoProfile()
  {
    // =============================================
    // Request
    // =============================================
    // Resumen
    CreateMap<DtoReq.CalOutResParametroDto, VO.CalOutResParametro>();
    CreateMap<DtoReq.CalOutResumenDto, VO.CalOutResumen>();

    // Detalle
    CreateMap<DtoReq.CalOutDetParametroDto, VO.CalOutDetParametro>();
    CreateMap<DtoReq.CalOutDetOtrosDto, VO.CalOutDetOtros>();
    CreateMap<DtoReq.CalOutDetalleDto, VO.CalOutDetalle>();

    // =============================================
    // Response
    // =============================================
    CreateMap<DtoRes.CalOutResumenDto, VO.CalOutResumenRef>();

  }
}
