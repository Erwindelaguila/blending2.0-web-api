using System;

namespace Function.Blending.Opt.Domain.ValueObjects;

// VO: inmutable, igualdad por valor (record), sin identidad propia
public sealed record CalInpFiltro(
  Guid? Id,
  Guid? EjecucionId,
  string CentroUbicacion,
  string CentroProduccion,
  string UbicacionAlmacen,
  bool MezclarTipoProduccion,
  string TipoProduccion,
  string? BorrarCalidades,
  bool QuitarRumasPH,
  string? AgregarRumasSerie,
  bool ConsiderarCadmio,
  DateTime? FechaCorteUtc,
  int? NumeroRuma,
  int? DivisionRuma,
  decimal ValorCadmioAlto
);
