using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Converters;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

public sealed record CalOutResumenDto
{
  public string? Grupo { get; init; }

  [JsonPropertyName("tonTotal")]
  public decimal? Toneladas { get; init; }

  [JsonPropertyName("fechaFabricacionNueva")]
  public string? NuevaFechaFabricacion { get; init; }

  [JsonPropertyName("calidadObjetivo")]
  public string? CodigoCalidadObjetivo { get; init; }

  [JsonPropertyName("calidadResultante")]
  public string? CodigoCalidadResultante { get; init; }

  public decimal? ValorInicial { get; init; }

  public decimal? ValorFinal { get; init; }

  public decimal? ValorAgregado { get; init; }

  public bool? Aceptado { get; init; } = false;

  [JsonPropertyName("parametros")]
  [JsonConverter(typeof(JsonObjectToResParametrosListConverter))]
  public IReadOnlyList<CalOutResParametroDto>? Parametros { get; init; }
}
