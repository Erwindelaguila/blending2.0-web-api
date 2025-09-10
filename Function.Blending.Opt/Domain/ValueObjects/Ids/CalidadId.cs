using System;

namespace Function.Blending.Opt.Domain.ValueObjects.Ids;

public readonly record struct CalidadId(Guid Value)
{
  public static CalidadId From(Guid value) =>
    value != Guid.Empty ? new CalidadId(value)
                        : throw new ArgumentException("CalidadId vacío");

  public override string ToString() => Value.ToString();

  // Implícita: VO -> Guid (para comparar/persistir)
  public static implicit operator Guid(CalidadId id) => id.Value;
  // Explícita: Guid -> VO (evita ambigüedad en '==')
  public static explicit operator CalidadId(Guid value) => new CalidadId(value);
}
