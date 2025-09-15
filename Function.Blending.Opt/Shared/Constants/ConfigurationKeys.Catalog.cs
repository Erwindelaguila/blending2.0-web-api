namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  // Catálogo por código
  public static class Catalog
  {
    public const string QualityExecutionStatusId = "Catalog:QualityExecutionStatus:Id";
    public const string LogisticExecutionStatusId = "Catalog:LogisticExecutionStatus:Id";
    
    public static class QualityExecutionStatus
    {
      public const string EnEjecucion = "Catalog:QualityExecutionStatus:EnEjecucion";
      public const string Procesado = "Catalog:QualityExecutionStatus:Procesado";
      public const string Cancelado = "Catalog:QualityExecutionStatus:Cancelado";
      public const string Error = "Catalog:QualityExecutionStatus:Error";
    }
    public static class LogisticExecutionStatus
    {
      public const string EnEjecucion = "Catalog:LogisticExecutionStatus:EnEjecucion";
      public const string Procesado = "Catalog:LogisticExecutionStatus:Procesado";
      public const string Cancelado = "Catalog:LogisticExecutionStatus:Cancelado";
      public const string Error = "Catalog:LogisticExecutionStatus:Error";
    }

    public static class QualityExecutionStatusProp
    {
      public const string ExposeColor = "Catalog:QualityExecutionStatus:Prop:ExposeColor";
      public const string ColorClave = "Catalog:QualityExecutionStatus:Prop:ColorClave";
    }

    public static class LogisticExecutionStatusProp
    {
      public const string ExposeColor = "Catalog:LogisticExecutionStatus:Prop:ExposeColor";
      public const string ColorClave = "Catalog:LogisticExecutionStatus:Prop:ColorClave";
    }

    public static class QualityExecutionStatusCache
    {
      public const string Enabled = "Catalog:QualityExecutionStatus:Cache:Enabled";
      public const string TtlSeconds = "Catalog:QualityExecutionStatus:Cache:TtlSeconds";
      public const string CacheNulls = "Catalog:QualityExecutionStatus:Cache:CacheNulls";
    }

    public static class LogisticExecutionStatusCache
    {
      public const string Enabled = "Catalog:LogisticExecutionStatus:Cache:Enabled";
      public const string TtlSeconds = "Catalog:LogisticExecutionStatus:Cache:TtlSeconds";
      public const string CacheNulls = "Catalog:LogisticExecutionStatus:Cache:CacheNulls";
    }

  }
}
