namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  /// <summary>
  /// Claves de configuración para cadenas de conexión.
  /// Mantén todos los nombres y rutas aquí para evitar "magic strings".
  /// </summary>
  public static class ConnectionStrings
  {
    // Nombre lógico que usa IConfiguration.GetConnectionString("BlendingDb")
    public const string BlendingDbName = "BlendingDb";

    // Ruta completa (fallback por si el proveedor no implementa GetConnectionString)
    public const string BlendingDb = "ConnectionStrings:BlendingDb";

    // Ruta completa (fallback desde Values)
    public const string SqlDb = "SqlConnectionString_BlendingDb";
  }
}
