using System.Collections.Generic;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests.Output;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;

// Pasamos a record nominal (propiedades init) para extender sin romper callers.
// La Function deserializa por propiedades (System.Text.Json), no por constructor.
public sealed record CompleteCalEjecucionRequest
{
  public Guid Id { get; init; }
  public Guid EstadoId { get; init; }
  public string? Mensaje { get; init; }

  // NUEVO (opcionales): todo el Output “de una sola vez”
  public IReadOnlyList<CalOutResumenDto>? Resumenes { get; init; }
  public IReadOnlyList<CalOutDetalleDto>? Detalles { get; init; }
}
