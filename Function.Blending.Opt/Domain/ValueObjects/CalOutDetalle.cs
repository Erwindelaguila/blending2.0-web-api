using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;

/// <summary>
/// Detalle de salida de Calidad para una ejecución (ruma/material).
/// </summary>
public sealed record CalOutDetalle(
  string? Grupo,
  string? Ruma,
  int? KilosUsados,
  int? Cantidad,
  string? Codigo,
  string? DescripcionMaterial,
  string? CentroUbicacion,
  string? AlmacenUbicacion,
  string? FechaContabilizacion,
  DateTime? NuevaFechaFabricacionUtc,
  bool? Aceptado,
  IReadOnlyList<CalOutDetParametro>? Parametros,
  IReadOnlyList<CalOutDetOtros>? Otros
);
