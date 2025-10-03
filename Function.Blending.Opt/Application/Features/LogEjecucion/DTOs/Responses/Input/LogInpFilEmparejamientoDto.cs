namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

public sealed record LogInpFilEmparejamientoDto
{
  public string Grupo { get; init; } = default!;
  public Guid ParametroId { get; init; }
  public decimal Valor { get; init; }
}
