using System.Text.Json;

namespace Function.Blending.Opt.Shared.Json;

/// <summary>
/// Política "lower camel case" semántica. Wrap del comportamiento por defecto de System.Text.Json
/// para dar un nombre explícito y auto-documentado en el código.
/// </summary>
public sealed class LowerCamelCaseNamingPolicy : JsonNamingPolicy
{
  // Reutilizamos la política nativa camelCase para asegurar compatibilidad.
  private static readonly JsonNamingPolicy Inner = CamelCase;

  public static readonly LowerCamelCaseNamingPolicy Instance = new();

  public override string ConvertName(string name)
  {
    // Delegamos en la implementación oficial (maneja acrónimos, etc.)
    return Inner.ConvertName(name);
  }
}
