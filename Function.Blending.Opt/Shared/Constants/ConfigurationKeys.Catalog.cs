namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  // Catálogo por código
  public static class Catalog
  {
    public const string QualityExecutionStatusId = "Catalog_QualityExecutionStatus_Id";
    public const string LogisticExecutionStatusId = "Catalog_LogisticExecutionStatus_Id";
    
    public static class QualityExecutionStatus
    {
      public const string EnEjecucion = "Catalog_QualityExecutionStatus_EnEjecucion";
      public const string Procesado = "Catalog_QualityExecutionStatus_Procesado";
      public const string Cancelado = "Catalog_QualityExecutionStatus_Cancelado";
      public const string Error = "Catalog_QualityExecutionStatus_Error";
    }
    public static class LogisticExecutionStatus
    {
      public const string EnEjecucion = "Catalog_LogisticExecutionStatus_EnEjecucion";
      public const string Procesado = "Catalog_LogisticExecutionStatus_Procesado";
      public const string Cancelado = "Catalog_LogisticExecutionStatus_Cancelado";
      public const string Error = "Catalog_LogisticExecutionStatus_Error";
    }

    public static class QualityExecutionStatusProp
    {
      public const string ExposeColor = "Catalog_QualityExecutionStatus_Prop_ExposeColor";
      public const string ColorClave = "Catalog_QualityExecutionStatus_Prop_ColorClave";
    }

    public static class LogisticExecutionStatusProp
    {
      public const string ExposeColor = "Catalog_LogisticExecutionStatus_Prop_ExposeColor";
      public const string ColorClave = "Catalog_LogisticExecutionStatus_Prop_ColorClave";
    }

    public static class QualityExecutionStatusCache
    {
      public const string Enabled = "Catalog_QualityExecutionStatus_Cache_Enabled";
      public const string TtlSeconds = "Catalog_QualityExecutionStatus_Cache_TtlSeconds";
      public const string CacheNulls = "Catalog_QualityExecutionStatus_Cache_CacheNulls";
    }

    public static class LogisticExecutionStatusCache
    {
      public const string Enabled = "Catalog_LogisticExecutionStatus_Cache_Enabled";
      public const string TtlSeconds = "Catalog_LogisticExecutionStatus_Cache_TtlSeconds";
      public const string CacheNulls = "Catalog_LogisticExecutionStatus_Cache_CacheNulls";
    }

  }
}
