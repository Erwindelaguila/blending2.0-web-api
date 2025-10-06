using System.Text.Json;

namespace Function.Blending.Core.Shared.Json;

/// <summary>
/// Pol�tica "lower camel case" sem�ntica. Wrap del comportamiento por defecto de System.Text.Json
/// para dar un nombre expl�cito y auto-documentado en el c�digo.
/// </summary>
public sealed class LowerCamelCaseNamingPolicy : JsonNamingPolicy
{
  // Reutilizamos la pol�tica nativa camelCase para asegurar compatibilidad.
  private static readonly JsonNamingPolicy Inner = CamelCase;

  public static readonly LowerCamelCaseNamingPolicy Instance = new();

  public override string ConvertName(string name)
  {
    // Delegamos en la implementaci�n oficial (maneja acr�nimos, etc.)
    return Inner.ConvertName(name);
  }
}
