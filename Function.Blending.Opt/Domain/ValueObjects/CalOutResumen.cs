namespace Function.Blending.Opt.Domain.ValueObjects;

/// <summary>
/// Resumen de salida de Calidad para una ejecución.
/// </summary>
public sealed record CalOutResumen(
  Guid? Id = default,
  Guid? EjecucionId = default,
  string? Grupo = null,
  decimal? Toneladas = null,
  string? NuevaFechaFabricacion = null,
  string? CodigoCalidadObjetivo = null,
  string? CodigoCalidadResultante = null,
  decimal? ValorInicial = null,
  decimal? ValorFinal = null,
  decimal? ValorAgregado = null,
  bool? Aceptado = null,
  IReadOnlyList<CalOutResParametro>? Parametros = null
);
