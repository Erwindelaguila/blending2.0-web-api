namespace Function.Blending.Opt.Infrastructure.Configuration.Options;

/// <summary>Opciones de caché para el catálogo de estados de Logística.</summary>
public sealed class EstadoLogisticaCacheOptions
{
  public bool Enabled { get; set; } = true;
  public int TtlSeconds { get; set; } = 300;
  public bool CacheNulls { get; set; } = false;
}
