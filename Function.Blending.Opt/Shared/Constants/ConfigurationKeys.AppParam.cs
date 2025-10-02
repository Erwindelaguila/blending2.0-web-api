namespace Function.Blending.Opt.Shared.Constants
{
  public static partial class ConfigurationKeys
  {
    public static class AppParam
    {
      public static class Cache
      {
        public const string Enabled = "AppParamCache_Enabled";
        public const string DefaultTtlSeconds = "AppParamCache_DefaultTtlSeconds";
        public const string CacheNulls = "AppParamCache_CacheNulls";
        public const string PrefixPerKeyTtlSeconds = "AppParamCache_PerKeyTtlSeconds_";
      }

      /// <summary>Claves de AppParam (tabla de par�metros de aplicaci�n).</summary>
      public static class Keys
      {
        public static class Quality
        {
          public const string ExecutionCodeFormat = "AppParam_QualityExecutionCodeFormat";
        }

        public static class Logistics
        {
          public const string ExecutionCodeFormat = "AppParam_LogisticExecutionCodeFormat";
          public const string NumeroMovimientos = "AppParam_LogisticNumeroMovimientos";
          public const string ValorDivision = "AppParam_LogisticValorDivision";
        }
      }
    }
  }
}
