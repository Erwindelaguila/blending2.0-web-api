namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Output;

public sealed record LogOutConDistribucionDto
{
  public Guid Id { get; init; }

  public Guid ContenedorId { get; init; }

  public string Ruma { get; init; } = null!;

  public int Valor { get; init; }
}