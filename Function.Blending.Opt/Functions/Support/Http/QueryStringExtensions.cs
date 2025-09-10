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
  }
}
