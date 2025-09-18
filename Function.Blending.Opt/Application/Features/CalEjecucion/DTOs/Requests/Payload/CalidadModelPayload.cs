using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Payload;

public sealed record CalidadModelPayload
{
  [JsonPropertyName("execution_id")]
  public Guid? EjecucionId { get; set; } = null;

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

  
  [JsonPropertyName("calidades_destino")]
  public IReadOnlyList<string>? CalidadesDestino { get; init; }
  
  [JsonPropertyName("parametros_homogenizado")]
  public IReadOnlyList<string>? Parametros { get; init; }
  
  [JsonPropertyName("df_parametros_salida")]
  public IReadOnlyList<IReadOnlyDictionary<string, object>>? CalidadParametros { get; init; }
  
  [JsonPropertyName("stock_rumas")]
  public IReadOnlyList<IReadOnlyDictionary<string, object>>? Rumas { get; init; }
}
