using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload.Internal;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Payload;

public sealed record LogisticaModelPayload
{
  public Guid? EjecucionId { get; set; } = null;
  public ObjetivoDto? Objetivo { get; init; }
  public IReadOnlyList<OfertaItemDto>? Oferta { get; init; }
  public IReadOnlyList<string>? Contenedores { get; init; }
  public IReadOnlyList<string>? ParametrosSeleccionados { get; init; }
  public IReadOnlyList<string>? IndiceOferta { get; init; }
  public IReadOnlyDictionary<string, decimal>? OfertaSacos { get; init; }
  public IReadOnlyList<CapacidadItemDto>? Capacidades { get; init; }
  public IReadOnlyDictionary<string, string>? Particiones { get; init; }
  public decimal? PesoContenedor { get; init; }
  public int? NroMovimientos { get; init; }
  public IReadOnlyDictionary<string, IReadOnlyDictionary<string, decimal>>? Emparejamientos { get; init; }
  public decimal? TiempoEspera { get; init; }
}