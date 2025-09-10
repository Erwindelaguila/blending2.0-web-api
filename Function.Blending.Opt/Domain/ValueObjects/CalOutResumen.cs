using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;

/// <summary>
/// Resumen de salida de Calidad para una ejecución.
/// </summary>
public sealed record CalOutResumen(
  string? Grupo,
  int? Toneladas,
  DateTime? NuevaFechaFabricacionUtc,
  string? CodigoCalidadObjetivo,
  string? CodigoCalidadResultante,
  int? ValorInicial,
  int? ValorFinal,
  int? ValorAgregado,
  bool? Aceptado,
  IReadOnlyList<CalOutResParametro>? Parametros
);
