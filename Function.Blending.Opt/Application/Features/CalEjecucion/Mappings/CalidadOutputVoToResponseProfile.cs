using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

/// <summary>
/// Mapeo simple de VO -> DTO de respuesta para Resumenes.
/// </summary>
public sealed class CalidadOutputVoToResponseProfile : Profile
{
  public CalidadOutputVoToResponseProfile()
  {
    CreateMap<VO.CalOutResumen, CalOutResumenDto>();
    CreateMap<VO.CalOutResParametro, CalOutResParametroDto>();

    CreateMap<VO.CalOutDetalle, CalOutDetalleDto>();
    CreateMap<VO.CalOutDetParametro, CalOutDetParametroDto>();
    CreateMap<VO.CalOutDetOtros, CalOutDetOtrosDto>();
  }
}
