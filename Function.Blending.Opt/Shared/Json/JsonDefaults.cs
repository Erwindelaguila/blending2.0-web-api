using System.Text.Json;
using System.Text.Json.Serialization;

namespace Function.Blending.Opt.Shared.Json;

/// <summary>
/// Opciones JSON compartidas para la web/API. Basadas en lower camel case,
/// case-insensitive y sin ignorar nulos (para respuestas explícitas).
/// </summary>
public static class JsonDefaults
{
  /// <summary>
  /// Opciones recomendadas para serialización/deserialización HTTP.
  /// </summary>
  public static readonly JsonSerializerOptions Web = new()
  {
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = LowerCamelCaseNamingPolicy.Instance,
    DictionaryKeyPolicy = LowerCamelCaseNamingPolicy.Instance,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never, // incluir nulls
    WriteIndented = false
  };

  public static T? Deserialize<T>(string json) =>
      JsonSerializer.Deserialize<T>(json, Web);

  public static string Serialize<T>(T value) =>
      JsonSerializer.Serialize(value, Web);
}
