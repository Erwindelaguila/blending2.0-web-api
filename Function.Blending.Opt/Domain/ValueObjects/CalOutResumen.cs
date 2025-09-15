using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;

/// <summary>
/// Resumen de salida de Calidad para una ejecución.
/// </summary>
public sealed record CalOutResumen(
  string? Grupo,
  decimal? Toneladas,
  DateTime? NuevaFechaFabricacion,
  string? CodigoCalidadObjetivo,
  string? CodigoCalidadResultante,
  decimal? ValorInicial,
  decimal? ValorFinal,
  decimal? ValorAgregado,
  bool? Aceptado,
  IReadOnlyList<CalOutResParametro>? Parametros
);
