using System;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Input;

// Record con *primary constructor* y nombres 1:1 con tu DDL
public sealed record CalInpFiltroDto(
  string CentroUbicacion,
  string CentroProduccion,
  string UbicacionAlmacen,
  bool MezclarTipoProduccion,
  string TipoProduccion,
  string? BorrarCalidades,
  bool QuitarRumasPH,
  bool AgruparRumas,
  bool ConsiderarCadmio,
  DateTimeOffset? FechaCorte, // DDL: datetime2
  int? NumeroRuma,
  int? DivisionRuma,
  decimal ValorCadmioAlto      // DDL: decimal(10,4)
);
