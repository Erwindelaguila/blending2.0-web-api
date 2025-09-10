using System.Collections.Generic;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

public sealed record LogInpOfertaDto(
  int Posicion,
  string Material,
  string Descripcion,
  int CantidadAsignadaVenta,
  int UnidadMedidaVenta,
  int CantidadAsignadaAlmacen,
  int UnidadMedidaAlmacen,
  decimal Tolerancia
)
{
  public IReadOnlyList<LogInpOfeParametroDto>? Parametros { get; init; }
}
