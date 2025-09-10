namespace Function.Blending.Opt.Shared.Constants
{
  /// <summary>
  /// Claves de configuración relacionadas a SysParam.
  /// Mantiene coherencia con tu patrón ConfigurationKeys.* ya existente.
  /// </summary>
  public static partial class ConfigurationKeys
  {
    public static class SysParam
    {
      /// <summary>
      /// Clave de configuración que contiene el nombre (string) de la sysparam-key
      /// que apunta al usuario de sistema en la tabla dbo.SysParam.
      /// Ej.: local.settings.json -> "SysParam:SystemUser": "SYS_USUARIO_SISTEMA"
      /// </summary>
      public const string SystemUser = "SysParam:SystemUser";

      // (Se usarán en el Paso 2 – caché)
      public static class Cache
      {
        public const string Enabled = "SysParamCache:Enabled";
        public const string DefaultTtlSeconds = "SysParamCache:DefaultTtlSeconds";
        public const string CacheNulls = "SysParamCache:CacheNulls";
        // PerKeyTtlSeconds:* se leerá por enumeración de config en el Paso 2.
        public const string PerKeyTtlPrefix = "SysParamCache:PerKeyTtlSeconds:";
      }
    }
  }
}
