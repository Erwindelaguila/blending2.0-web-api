using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using Dto = Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class CalidadOutputDtoToVoProfile : Profile
{
  public CalidadOutputDtoToVoProfile()
  {
    // Resumen
    CreateMap<Dto.CalOutResParametroDto, VO.CalOutResParametro>();
    CreateMap<Dto.CalOutResumenDto, VO.CalOutResumen>();

    // Detalle
    CreateMap<Dto.CalOutDetParametroDto, VO.CalOutDetParametro>();
    CreateMap<Dto.CalOutDetOtrosDto, VO.CalOutDetOtros>();
    CreateMap<Dto.CalOutDetalleDto, VO.CalOutDetalle>();
  }
}
