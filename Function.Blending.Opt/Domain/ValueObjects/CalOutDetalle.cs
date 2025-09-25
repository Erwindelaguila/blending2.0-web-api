using System;
using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;

/// <summary>
/// Detalle de salida de Calidad para una ejecución (ruma/material).
/// </summary>
public sealed record CalOutDetalle(
  Guid? Id = default,
  Guid? EjecucionId = default,
  string? Grupo = null,
  string? Ruma = null,
  decimal? KilosUsados = null,
  decimal? Cantidad = null,
  string? Codigo = null,
  string? DescripcionMaterial = null,
  string? CentroUbicacion = null,
  string? AlmacenUbicacion = null,
  string? FechaContabilizacion = null,
  string? FechaFabricacion = null,
  string? NuevaFechaFabricacion = null,
  bool? Aceptado = null,
  IReadOnlyList<CalOutDetParametro>? Parametros = null,
  IReadOnlyList<CalOutDetOtros>? Otros = null
);
