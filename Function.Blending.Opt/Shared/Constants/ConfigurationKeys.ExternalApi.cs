namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static class ExternalServices
  {
    public static string EnableQualityModel => "ExternalService_EnableQualityModel";
    public static string EnableLogisticsModel => "ExternalService_EnableLogisticsModel";

    public static class QualityModel
    {
      public const string BaseUrl = "External_QualityModel_BaseUrl";
      public const string StartPath = "External_QualityModel_StartPath";
      public const string TimeoutSeconds = "External_QualityModel_TimeoutSeconds";
      public const string ApiKey = "External_QualityModel_ApiKey";
    }

    public static class LogisticsModel
    {
      public const string BaseUrl = "External_LogisticsModel_BaseUrl";
      public const string StartPath = "External_LogisticsModel_StartPath";
      public const string TimeoutSeconds = "External_LogisticsModel_TimeoutSeconds";
      public const string ApiKey = "External_LogisticsModel_ApiKey";
    }
  }
}
