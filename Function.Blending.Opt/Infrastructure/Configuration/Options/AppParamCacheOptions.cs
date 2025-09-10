using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Configuration.Options;

public sealed class AppParamCacheOptions
{
  public const string SectionName = "AppParamCache";

  /// <summary>Habilita/deshabilita cacheo de parámetros.</summary>
  public bool Enabled { get; set; } = true;

  /// <summary>TTL por defecto en segundos (si no hay override por clave).</summary>
  public int DefaultTtlSeconds { get; set; } = 300; // 5 min

  /// <summary>Si true, también cachea "no encontrado" (null/”no activo”).</summary>
  public bool CacheNulls { get; set; } = false;

  /// <summary>Overrides por clave. Ej: { "APP_CAL_CODIGO_FORMAT": 1800 }</summary>
  public Dictionary<string, int> PerKeyTtlSeconds { get; set; } = [];
}
