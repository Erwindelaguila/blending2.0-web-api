namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;

public sealed record LogInpDemandaDto(
  int Posicion,
  string Material,
  string Descripcion,
  decimal CantidadAsignadaVenta,
  string UnidadMedidaVenta,
  decimal CantidadAsignadaAlmacen,
  string UnidadMedidaAlmacen,
  decimal Tolerancia
)
{
  public IReadOnlyList<LogInpDemParametroDto>? Parametros { get; init; }
}
