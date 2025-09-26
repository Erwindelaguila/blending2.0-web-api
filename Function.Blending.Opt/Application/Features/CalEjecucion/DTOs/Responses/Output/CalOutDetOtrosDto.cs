namespace Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Output;

public sealed class CalOutDetOtrosDto
{
  public Guid Id { get; init; }

  public Guid DetalleId { get; init; }

  public string Codigo { get; init; } = default!;
  public string Valor { get; init; } = default!;
}
