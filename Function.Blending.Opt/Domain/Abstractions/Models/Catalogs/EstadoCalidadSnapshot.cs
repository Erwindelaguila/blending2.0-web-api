namespace Function.Blending.Opt.Domain.Abstractions.Models.Catalogs;

/// <summary>
/// Referencia inmutable a un Estado de Calidad (catálogo externo).
/// Value Object con igualdad por Id (solo Id participa en la igualdad).
/// </summary>
public sealed record EstadoCalidadSnapshot(Guid Id)
{
  public string Nombre { get; init; } = "";
  public string? Color { get; init; }
}
