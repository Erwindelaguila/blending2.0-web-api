namespace Function.Blending.Opt.Shared.Options.Time;

public sealed class TimeZoneOptions
{
  // Id de zona horaria (Windows o IANA; ver implementación)
  // Ej: "America/Lima" (IANA) o "SA Pacific Standard Time" (Windows)
  public string? TimeZoneId { get; set; }
  // Id de zona horaria específica para Windows
  public string? WindowsTimeZoneId { get; set; }
  // Id de zona horaria específica para IANA
  public string? IanaTimeZoneId { get; set; }
  /// <summary>Si true, cualquier error al resolver la zona horaria lanza excepción.</summary>
  public bool EnforceValidTimeZone { get; set; } = false;
}
