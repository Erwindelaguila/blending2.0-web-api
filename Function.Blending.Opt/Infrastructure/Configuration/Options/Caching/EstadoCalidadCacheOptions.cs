namespace Function.Blending.Opt.Infrastructure.Configuration.Options;

public sealed class EstadoCalidadCacheOptions
{
  /// <summary>Habilita/Deshabilita caché.</summary>
  public bool Enabled { get; set; } = true;

  /// <summary>TTL por entrada (segundos) para GetById / items de GetByIds.</summary>
  public int TtlSeconds { get; set; } = 300;

  /// <summary>Si true, también cachea faltantes (null) para evitar golpear DB repetidamente.</summary>
  public bool CacheNulls { get; set; } = false;
}
