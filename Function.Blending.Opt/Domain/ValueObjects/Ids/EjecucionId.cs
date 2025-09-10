using System;

namespace Function.Blending.Opt.Domain.ValueObjects.Ids;

public readonly record struct EjecucionId(Guid Value)
{
  public static EjecucionId From(Guid value) =>
    value != Guid.Empty ? new EjecucionId(value)
                        : throw new ArgumentException("EjecucionId vacío");

  public override string ToString() => Value.ToString();

  // Implícita: VO -> Guid (comparaciones / llamadas a APIs Guid)
  public static implicit operator Guid(EjecucionId id) => id.Value;
  // Explícita: Guid -> VO (evita ambigüedad en '==')
  public static explicit operator EjecucionId(Guid value) => new EjecucionId(value);
}
