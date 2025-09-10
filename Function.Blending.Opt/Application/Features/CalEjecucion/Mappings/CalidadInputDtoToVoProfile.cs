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
        .ForCtorParam(nameof(CalInpFiltro.FechaCorteUtc),
          opt => opt.MapFrom(src => src.FechaCorte.HasValue
            ? src.FechaCorte.Value.UtcDateTime
            : (DateTime?)null));

      // === cambio aquí: usar VO de IDs ===
      CreateMap<CalInpParametroDto, CalInpParametro>()
        .ConstructUsing(src => new CalInpParametro(
          CalidadId.From(src.CalidadId),
          ParametroId.From(src.ParametroId),
          src.Valor
        ));
    }
  }
}
