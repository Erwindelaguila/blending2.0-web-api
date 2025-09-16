using System;
using System.Linq;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;
using TimeZoneConverter;

namespace Function.Blending.Opt.Infrastructure.Services.Time;

public sealed class TimeZoneService(IConfiguration cfg) : ITimeZoneService
{
  private readonly TimeZoneInfo _tz = Resolve(cfg) ?? TimeZoneInfo.Utc;

  public string CurrentTimeZoneId => _tz.Id;

  public DateTimeOffset ToLocal(DateTimeOffset utc)
  {
    if (utc.Offset != TimeSpan.Zero) utc = utc.ToUniversalTime();
    var local = TimeZoneInfo.ConvertTime(utc.UtcDateTime, _tz);
    return new DateTimeOffset(local, _tz.GetUtcOffset(local));
  }

  // ---------- helpers ----------
  private static TimeZoneInfo? Resolve(IConfiguration cfg)
  {
    // Prioridad: genérica -> específica Windows -> específica IANA
    var id = FirstNonEmpty(
      cfg[ConfigurationKeys.Time.TimeZoneId],
      cfg[ConfigurationKeys.Time.WindowsTimeZoneId],
      cfg[ConfigurationKeys.Time.IanaTimeZoneId]
    );

    if (string.IsNullOrWhiteSpace(id)) return null;

    try { return TZConvert.GetTimeZoneInfo(id); }
    catch { return null; } // fallback a UTC lo maneja el ctor
  }

  private static string? FirstNonEmpty(params string?[] values)
    => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
}
