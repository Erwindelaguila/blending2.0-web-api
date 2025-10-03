namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Output;

public sealed record LogOutConComposicionDto
{
  public Guid Id { get; init; }

  public Guid ContenedorId { get; init; }

  public string CodigoParametro { get; init; } = null!;

  public decimal Valor { get; init; }
}