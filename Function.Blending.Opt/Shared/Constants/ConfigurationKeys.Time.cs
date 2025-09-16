using System.Runtime.ConstrainedExecution;
using System.Security.Policy;

namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static class Time
  {
    // Id de zona horaria (Windows o IANA; ver implementación)
    // Ej: "America/Lima" (IANA) o "SA Pacific Standard Time" (Windows)
    public const string TimeZoneId = "Time_TimeZoneId";
    // Id de zona horaria específica para Windows
    public const string WindowsTimeZoneId = "Time_WindowsTimeZoneId";
    // Id de zona horaria específica para IANA
    public const string IanaTimeZoneId = "Time_IanaTimeZoneId";
  }
}
