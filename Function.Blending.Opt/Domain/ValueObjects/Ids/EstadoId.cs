namespace Function.Blending.Opt.Domain.ValueObjects.Ids;

public readonly record struct EstadoId(Guid Value)
{
  public static EstadoId From(Guid value) =>
    value != Guid.Empty ? new EstadoId(value)
                        : throw new ArgumentException("EstadoId vacío");
  public override string ToString() => Value.ToString();

  // Implícita: VO -> Guid (para comparar y pasar a APIs Guid)
  public static implicit operator Guid(EstadoId id) => id.Value;

  // Explícita: Guid -> VO (evita ambigüedad en '==')
  public static explicit operator EstadoId(Guid value) => new EstadoId(value);
}
