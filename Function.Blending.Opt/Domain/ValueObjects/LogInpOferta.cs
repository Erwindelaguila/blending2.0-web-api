using System.Collections.Generic;

namespace Function.Blending.Opt.Domain.ValueObjects;
public sealed record LogInpOferta(
  int Posicion,
  string Material,
  string Descripcion,
  int CantidadAsignadaVenta,
  int UnidadMedidaVenta,
  int CantidadAsignadaAlmacen,
  int UnidadMedidaAlmacen,
  decimal Tolerancia,
  IReadOnlyList<LogInpOfeParametro>? Parametros
);
