using System;

namespace Function.Blending.Opt.Domain.ValueObjects.Ids;

public readonly record struct ParametroId(Guid Value)
{
  public static ParametroId From(Guid value) =>
    value != Guid.Empty ? new ParametroId(value)
                        : throw new ArgumentException("ParametroId vacío");

  public override string ToString() => Value.ToString();

  // Implícita: VO -> Guid
  public static implicit operator Guid(ParametroId id) => id.Value;
  // Explícita: Guid -> VO
  public static explicit operator ParametroId(Guid value) => new ParametroId(value);
}
