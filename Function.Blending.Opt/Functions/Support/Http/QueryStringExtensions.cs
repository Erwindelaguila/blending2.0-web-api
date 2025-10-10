using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Opt.Functions.Support.Http
{
  /// <summary>
  /// Extensiones para leer y convertir parámetros de query de forma consistente.
  /// </summary>
  public static class QueryStringExtensions
  {
    /// <summary>Devuelve el query ya parseado del request.</summary>
    public static NameValueCollection GetQuery(this HttpRequestData req)
      => HttpUtility.ParseQueryString(req.Url.Query);

    /// <summary>Obtiene un entero o devuelve el valor por defecto.</summary>
    public static int GetIntOrDefault(this NameValueCollection qs, string key, int @default)
      => int.TryParse(qs[key], out var v) ? v : @default;

    /// <summary>Obtiene un string o null si viene vacío/solo espacios.</summary>
    public static string? GetStringOrNull(this NameValueCollection qs, string key)
    {
      var s = qs[key];
      return string.IsNullOrWhiteSpace(s) ? null : s;
    }

    /// <summary>Obtiene un Guid nullable.</summary>
    public static Guid? GetGuidOrNull(this NameValueCollection qs, string key)
      => Guid.TryParse(qs[key], out var g) ? g : null;

    /// <summary>Obtiene un boolean nullable.</summary>
    public static bool? GetBoolOrNull(this NameValueCollection qs, string key)
      => bool.TryParse(qs[key], out var b) ? b : null;

    /// <summary>
    /// Intenta parsear fecha/hora asumiendo/ajustando a UTC. Acepta varios aliases.
    /// Ej.: <c>qs.GetUtcDateTime("creadoDelUtc", "creadoDel")</c>
    /// </summary>
    public static DateTime? GetUtcDateTime(this NameValueCollection qs, params string[] keys)
    {
      foreach (var k in keys)
      {
        var raw = qs[k];
        if (string.IsNullOrWhiteSpace(raw)) continue;

        if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dto))
        {
          return dto.UtcDateTime;
        }
      }
      return null;
    }

    // ======================================================================
    // ENTRIES helpers (genérico: key + allowed + synonyms + separadores)
    /* Ejemplo de uso:
     // expand genérico
      var expand = req.GetEntries(
        key: "expand",
        allowed: new[] { "input", "output" },
        synonyms: new Dictionary<string, string> { ["in"] = "input", ["out"] = "output" },
        separators: new[] { ',' } // opcional; por defecto ','
      );

      if (expand.Contains("input")) { cargar input }
      if (req.HasEntry("expand", "output", allowed: new[] { "input", "output" })) {  ...  }
    */
    // ======================================================================

    /// <summary>
    /// Obtiene un set (case-insensitive) con los valores de un parámetro multivalor del query (p.ej. ?expand=...).
    /// </summary>
    /// <param name="req">HttpRequestData.</param>
    /// <param name="key">Nombre del parámetro (obligatorio).</param>
    /// <param name="allowed">Opcional: valores permitidos (case-insensitive). Si se especifica, los no permitidos se descartan.</param>
    /// <param name="synonyms">Opcional: mapa de sinónimos → valor canónico (case-insensitive). Ej.: {"in":"input","out":"output"}.</param>
    /// <param name="separators">Separadores (por defecto: ',').</param>
    /// <param name="splitOptions">Opciones de split (por defecto: RemoveEmptyEntries | TrimEntries).</param>
    /// <param name="unquoteDoubleQuotes">Si true, elimina comillas dobles envolventes.</param>
    public static HashSet<string> GetEntries(
      this HttpRequestData req,
      string key,
      IEnumerable<string>? allowed = null,
      IReadOnlyDictionary<string, string>? synonyms = null,
      IEnumerable<char>? separators = null,
      StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries,
      bool unquoteDoubleQuotes = true)
      => req.GetQuery().GetEntries(key, allowed, synonyms, separators, splitOptions, unquoteDoubleQuotes);

    /// <summary>
    /// Obtiene un set (case-insensitive) con los valores de un parámetro multivalor del query (p.ej. ?expand=...).
    /// </summary>
    /// <param name="qs">Query string parseado.</param>
    /// <param name="key">Nombre del parámetro (obligatorio).</param>
    /// <param name="allowed">Opcional: valores permitidos (case-insensitive). Si se especifica, los no permitidos se descartan.</param>
    /// <param name="synonyms">Opcional: mapa de sinónimos → valor canónico (case-insensitive). Ej.: {"in":"input","out":"output"}.</param>
    /// <param name="separators">Separadores (por defecto: ',').</param>
    /// <param name="splitOptions">Opciones de split (por defecto: RemoveEmptyEntries | TrimEntries).</param>
    /// <param name="unquoteDoubleQuotes">Si true, elimina comillas dobles envolventes.</param>
    public static HashSet<string> GetEntries(
      this NameValueCollection qs,
      string key,
      IEnumerable<string>? allowed = null,
      IReadOnlyDictionary<string, string>? synonyms = null,
      IEnumerable<char>? separators = null,
      StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries,
      bool unquoteDoubleQuotes = true)
    {
      var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

      if (string.IsNullOrWhiteSpace(key))
        return result;

      var raw = qs[key];
      if (string.IsNullOrWhiteSpace(raw))
        return result;

      // Separadores
      var seps = (separators is null || !separators.Any()) ? new[] { ',' } : separators.ToArray();

      // Normalizar allowed y synonyms (case-insensitive)
      HashSet<string>? allowedSet = null;
      if (allowed is not null)
        allowedSet = new HashSet<string>(allowed, StringComparer.OrdinalIgnoreCase);

      Dictionary<string, string>? syn = null;
      if (synonyms is not null)
        syn = synonyms.ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

      foreach (var token in raw.Split(seps, splitOptions))
      {
        var t = token;

        // Quitar comillas dobles exteriores si aplica
        if (unquoteDoubleQuotes && t.Length >= 2 && t[0] == '"' && t[^1] == '"')
          t = t.Substring(1, t.Length - 2);

        // Aplicar sinónimo → valor canónico
        if (syn is not null && syn.TryGetValue(t, out var mapped))
          t = mapped;

        // Filtrar por allowed si corresponde
        if (allowedSet is not null && !allowedSet.Contains(t))
          continue;

        if (!string.IsNullOrWhiteSpace(t))
          result.Add(t);
      }

      return result;
    }

    /// <summary>
    /// Indica si el parámetro multivalor (p.ej. ?expand=...) contiene un valor específico (case-insensitive).
    /// </summary>
    public static bool HasEntry(
      this HttpRequestData req,
      string key,
      string value,
      IEnumerable<string>? allowed = null,
      IReadOnlyDictionary<string, string>? synonyms = null,
      IEnumerable<char>? separators = null,
      StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries,
      bool unquoteDoubleQuotes = true)
      => req.GetEntries(key, allowed, synonyms, separators, splitOptions, unquoteDoubleQuotes).Contains(value);

    /// <summary>
    /// Indica si el parámetro multivalor (p.ej. ?expand=...) contiene un valor específico (case-insensitive).
    /// </summary>
    public static bool HasEntry(
      this NameValueCollection qs,
      string key,
      string value,
      IEnumerable<string>? allowed = null,
      IReadOnlyDictionary<string, string>? synonyms = null,
      IEnumerable<char>? separators = null,
      StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries,
      bool unquoteDoubleQuotes = true)
      => qs.GetEntries(key, allowed, synonyms, separators, splitOptions, unquoteDoubleQuotes).Contains(value);
  }
}