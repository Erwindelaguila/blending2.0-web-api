using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;

/// <summary>
/// Detalle de salida de Calidad para una ejecución (ruma/material).
/// </summary>
public sealed record CalOutDetalle(
  string? Grupo,
  string? Ruma,
  decimal? KilosUsados,
  decimal? Cantidad,
  string? Codigo,
  string? DescripcionMaterial,
  string? CentroUbicacion,
  string? AlmacenUbicacion,
  string? FechaContabilizacion,
  string? FechaFabricacion,
  string? NuevaFechaFabricacion,
  bool? Aceptado,
  IReadOnlyList<CalOutDetParametro>? Parametros,
  IReadOnlyList<CalOutDetOtros>? Otros
);
