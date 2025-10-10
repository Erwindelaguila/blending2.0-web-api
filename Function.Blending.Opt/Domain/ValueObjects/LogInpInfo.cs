namespace Function.Blending.Opt.Domain.ValueObjects;

public sealed record LogInpInfo
{
  public string Contrato { get; init; } = default!;
  public string PedidoVenta { get; init; } = default!;
  public DateTime FechaCarguio { get; init; } = default!;
  public string PlantaCodigo { get; init; } = default!;
  public string PlantaDescripcion { get; init; } = default!;
  public string AlmacenCodigo { get; init; } = default!;
  public string AlmacenDescripcion { get; init; } = default!;
  public string Cliente { get; init; } = default!;
  public string Asistente { get; init; } = default!;
  public string Supervisora { get; init; } = default!;
  public string PaisDestino { get; init; } = default!;
  public decimal CantidadRuma { get; init; } = default!;
  public string UnidadMedidaRuma { get; init; } = default!;
  public int NumeroMovimientos { get; set; } = default!;
}
