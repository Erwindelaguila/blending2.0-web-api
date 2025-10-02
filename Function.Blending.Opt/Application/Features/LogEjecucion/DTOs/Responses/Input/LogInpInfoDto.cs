namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpInfoDto
{
  public string Contrato { get; init; } = default!;
  public string PedidoVenta { get; init; } = default!;
  public DateTime FechaCarguio { get; init; }
  public string PlantaCodigo { get; init; } = default!;
  public string PlantaDescripcion { get; init; } = default!;
  public string AlmacenCodigo { get; init; } = default!;
  public string AlmacenDescripcion { get; init; } = default!;
  public string Cliente { get; init; } = default!;
  public string Asistente { get; init; } = default!;
  public string Supervisora { get; init; } = default!;
  public string PaisDestino { get; init; } = default!;
  public int CantidadRuma { get; init; }
  public string UnidadMedidaRuma { get; init; } = default!;
  public int NumeroMovimientos { get; init; }
}
