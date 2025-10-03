using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;
using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Domain.ValueObjects.Ids;
using System;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings
{
  public sealed class CalidadInputDtoToVoProfile : Profile
  {
    public CalidadInputDtoToVoProfile()
    {
      // Para records posicionales, usa constructor mapping.
      // Solo FechaCorteUtc no coincide por nombre, así que se mapea explícitamente.
      CreateMap<CalInpFiltroDto, CalInpFiltro>()
        .ForCtorParam(nameof(CalInpFiltro.Id), m => m.MapFrom(s => (Guid?)null))
        .ForCtorParam(nameof(CalInpFiltro.EjecucionId), m => m.MapFrom(s => (Guid?)null))
        .ForCtorParam(nameof(CalInpFiltro.FechaCorteUtc), opt => opt.MapFrom(src => src.FechaCorte.HasValue ? src.FechaCorte.Value.UtcDateTime : (DateTime?)null));

      // === cambio aquí: usar VO de IDs ===
      CreateMap<CalInpParametroDto, CalInpParametro>()
        .ForCtorParam(nameof(CalInpParametro.Id), m => m.MapFrom(s => (Guid?)null))
        .ForCtorParam(nameof(CalInpParametro.EjecucionId), m => m.MapFrom(s => (Guid?)null))
        .ForCtorParam(nameof(CalInpParametro.CalidadId), m => m.MapFrom(s => new CalidadId(s.CalidadId)))
        .ForCtorParam(nameof(CalInpParametro.ParametroId), m => m.MapFrom(s => new ParametroId(s.ParametroId)));
    }
  }
}
