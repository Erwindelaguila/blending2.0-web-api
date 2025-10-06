using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Output;

public sealed record LogOutContenedorDto
{
  public Guid Id { get; init; }

  public Guid EjecucionId { get; init; }

  public string Contenedor { get; init; } = null!;

  public string? Grupo { get; init; }

  public Guid? CreadoPorId { get; init; }

  public DateTime CreadoEl { get; init; }

  public Guid? ModificadoPorId { get; init; }

  public DateTime? ModificadoEl { get; init; }

  [JsonPropertyName("composicion")]
  public IReadOnlyList<LogOutConComposicionDto>? LogOutConComposicion { get; init; } = [];

  [JsonPropertyName("distribucion")]
  public IReadOnlyList<LogOutConDistribucionDto>? LogOutConDistribucion { get; init; } = [];
}
