using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Converters;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

public sealed record CalOutDetalleDto
{
  [JsonPropertyName("grupo")]
  public string? Grupo { get; init; }

  [JsonPropertyName("rumaNro")]
  public string? Ruma { get; init; }

  public decimal? KilosUsados { get; init; }
  public decimal? Cantidad { get; init; }
  public string? Codigo { get; init; }
  public string? DescripcionMaterial { get; init; }

  public string? CentroUbicacion { get; init; }

  public string? AlmacenUbicacion { get; init; }

  public string? FechaContabilizacion { get; init; }

  public string? FechaFabricacion { get; init; }

  [JsonPropertyName("fechaFabricacionNueva")]
  public string? NuevaFechaFabricacion { get; init; }

  public bool? Aceptado { get; init; } = false;

  [JsonPropertyName("parametros")]
  [JsonConverter(typeof(JsonObjectToDetParametrosListConverter))]
  public IReadOnlyList<CalOutDetParametroDto>? Parametros { get; init; }

  [JsonPropertyName("otrosParam")]
  [JsonConverter(typeof(JsonObjectToOtrosListConverter))]
  public IReadOnlyList<CalOutDetOtrosDto>? Otros { get; init; }
}
