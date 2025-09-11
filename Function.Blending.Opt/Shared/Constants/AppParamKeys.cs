namespace Function.Blending.Opt.Shared.Constants;

public sealed class AppParamKeys
{
  public static class Cache
  { 
    public const string Enabled = "AppParamCache:Enabled";
    public const string DefaultTtlSeconds = "AppParamCache:DefaultTtlSeconds";
    public const string CacheNulls = "AppParamCache:CacheNulls";
    public const string PrefixPerKeyTtlSeconds = "AppParamCache:PerKeyTtlSeconds:";
  }

  /// <summary>Claves de AppParam (tabla de parámetros de aplicación).</summary>
  public static class Keys
  {
    public static class Quality
    {
      public const string ExecutionCodeFormat = "AppParam:QualityExecutionCodeFormat";
    }

    public static class Logistics
    {
      public const string ExecutionCodeFormat = "AppParam:LogisticExecutionCodeFormat";
    }
  }
}
