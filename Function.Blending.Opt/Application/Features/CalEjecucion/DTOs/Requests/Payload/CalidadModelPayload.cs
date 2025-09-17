using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Payload;

public sealed record CalidadModelPayload
{
  [JsonPropertyName("tipo_homogenizado")]
  public string? TipoHomogenizado { get; init; }

  [JsonPropertyName("planta_mezclado")]
  public string? Planta { get; init; }

  [JsonPropertyName("incluir_cadmio")]
  public string? IncluirCadmio { get; init; }

  [JsonPropertyName("valor_cadmio_alto")]
  public decimal? ValorCadmioAlto { get; init; }

  [JsonPropertyName("cantidad_rumas")]
  public decimal? CantidadRumas { get; init; }

  [JsonPropertyName("ton_multiplos")]
  public decimal? DivisionRumas { get; init; }



  public IReadOnlyList<CalInpParametroDto>? Parametros { get; init; }
}
