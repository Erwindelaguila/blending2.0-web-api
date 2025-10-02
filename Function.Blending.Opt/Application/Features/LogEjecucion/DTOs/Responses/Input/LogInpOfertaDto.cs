namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpOfertaDto
{
  public int Posicion { get; init; }
  public string Material { get; init; } = default!;
  public string Descripcion { get; init; } = default!;
  public int CantidadAsignadaVenta { get; init; }
  public string? UnidadMedidaVenta { get; init; }
  public int CantidadAsignadaAlmacen { get; init; }
  public string? UnidadMedidaAlmacen { get; init; }
  public decimal Tolerancia { get; init; }

  public IReadOnlyList<LogInpOfeParametroDto>? Parametros { get; init; }
}
