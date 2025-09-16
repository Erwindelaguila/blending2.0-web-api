using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Response.Output;
using System;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;

public sealed class CalEjecucionResponse
{
  public Guid Id { get; init; }
  public Guid PlantaId { get; init; }
  public string? Codigo { get; init; }
  public string? Mensaje { get; init; }
  public DateTime CreadoEl { get; init; } // UTC

  public EstadoResponse? Estado { get; init; }

  [JsonPropertyName("grupos")]
  public IReadOnlyList<CalOutResumenDto>? Resumenes { get; init; }
}
