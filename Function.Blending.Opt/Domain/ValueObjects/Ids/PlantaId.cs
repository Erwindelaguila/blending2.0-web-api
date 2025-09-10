namespace Function.Blending.Opt.Domain.ValueObjects.Ids;

public readonly record struct PlantaId(Guid Value)
{
  public static PlantaId From(Guid value) =>
    value != Guid.Empty ? new PlantaId(value)
                        : throw new ArgumentException("PlantaId vacío");
  public override string ToString() => Value.ToString();

  // Implícita: VO -> Guid
  public static implicit operator Guid(PlantaId id) => id.Value;

  // Explícita: Guid -> VO
  public static explicit operator PlantaId(Guid value) => new PlantaId(value);
}
