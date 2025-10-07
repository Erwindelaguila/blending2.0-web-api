using Function.Blending.Opt.Domain.Abstractions.Services;
using TimeZoneConverter;

namespace Function.Blending.Opt.Infrastructure.Services.Time;

public sealed class TimeZoneResolver : ITimeZoneResolver
{
  public TimeZoneInfo? TryResolve(string? timeZoneId)
  {
    if (string.IsNullOrWhiteSpace(timeZoneId)) return null;
    try { return TZConvert.GetTimeZoneInfo(timeZoneId); }
    catch { return null; }
  }
}
