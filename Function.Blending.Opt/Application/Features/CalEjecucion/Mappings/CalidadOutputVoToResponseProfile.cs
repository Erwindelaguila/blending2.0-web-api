using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using DtoRes = Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Response.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

/// <summary>
/// Mapeo simple de VO -> DTO de respuesta para Resumenes.
/// </summary>
public sealed class CalidadOutputVoToResponseProfile : Profile
{
  public CalidadOutputVoToResponseProfile()
  {
    // Mapea por nombre. No incluye Parametros (no lo necesitas en GetById).
    CreateMap<VO.CalOutResumen, DtoRes.CalOutResumenDto>();
  }
}
