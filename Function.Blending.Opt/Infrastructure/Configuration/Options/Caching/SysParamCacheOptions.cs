using System.Collections.Generic;

namespace Function.Blending.Opt.Infrastructure.Configuration.Options
{
  /// <summary>Caché para lectura de SysParam (similar a AppParam).</summary>
  public sealed class SysParamCacheOptions
  {
    /// <summary>Habilita/deshabilita cacheo de sysparams.</summary>
    public bool Enabled { get; set; } = true;

    /// <summary>TTL por defecto en segundos (si no hay override por clave).</summary>
    public int DefaultTtlSeconds { get; set; } = 300;

    /// <summary>Si true, también cachea "no encontrado" (null) por TTL.</summary>
    public bool CacheNulls { get; set; } = false;

    /// <summary>TTL específico por key de SysParam (e.g. "SYS_USUARIO_SISTEMA" = 3600).</summary>
    public Dictionary<string, int> PerKeyTtlSeconds { get; set; } = new();
  }
}
