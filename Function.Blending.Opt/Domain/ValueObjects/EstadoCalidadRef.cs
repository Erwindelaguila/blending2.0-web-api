namespace Function.Blending.Opt.Domain.ValueObjects;

/// <summary>
/// Referencia inmutable a un Estado de Calidad (catálogo externo).
/// Value Object con igualdad por Id (solo Id participa en la igualdad).
/// </summary>
public sealed record EstadoCalidadRef(Guid Id)
{
  public string Nombre { get; init; } = "";
  public string? Color { get; init; }
}
