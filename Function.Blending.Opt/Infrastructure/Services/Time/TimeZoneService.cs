using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Exceptions;
using Function.Blending.Opt.Shared.Options.Time;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TimeZoneConverter;

namespace Function.Blending.Opt.Infrastructure.Services.Time;

public sealed class TimeZoneService : ITimeZoneService
{
  private readonly ILogger<TimeZoneService> _logger;
  private readonly bool _enforce;
  private readonly Lazy<TimeZoneInfo> _tzLazy;
  private readonly string? _configuredId;

  public TimeZoneService(IOptions<TimeZoneOptions> opt, ILogger<TimeZoneService> logger)
  {
    _logger = logger;
    _enforce = opt.Value.EnforceValidTimeZone;

    // Tomamos el primer ID configurado (tu prioridad: genérico -> Windows -> IANA)
    _configuredId = FirstNonEmpty(
      opt.Value.TimeZoneId,
      opt.Value.WindowsTimeZoneId,
      opt.Value.IanaTimeZoneId
    );

    // Lazy: resolvemos cuando se use por primera vez (y ahí lanzamos si corresponde)
    _tzLazy = new Lazy<TimeZoneInfo>(() => ResolveOrThrow(_configuredId, _enforce, _logger));
  }

  public string CurrentTimeZoneId => _tzLazy.Value.Id;

  public DateTimeOffset ToLocal(DateTimeOffset utc)
  {
    // Forzamos la resolución (si falla y _enforce=true, aquí se lanzará)
    var tz = _tzLazy.Value;

    if (utc.Offset != TimeSpan.Zero)
    {
      _logger.LogDebug("ToLocal recibió un DateTimeOffset no-UTC (offset {Offset}). Convirtiendo a UTC.", utc.Offset);
      utc = utc.ToUniversalTime();
    }

    var local = TimeZoneInfo.ConvertTime(utc.UtcDateTime, tz);
    return new DateTimeOffset(local, tz.GetUtcOffset(local));
  }

  // ---------- helpers ----------
  private static TimeZoneInfo ResolveOrThrow(string? id, bool enforce, ILogger logger)
  {
    if (string.IsNullOrWhiteSpace(id))
    {
      if (enforce)
      {
        throw new TimeZoneConfigurationException("No se encontró configuración de zona horaria (TimeZoneId/WindowsTimeZoneId/IanaTimeZoneId).");
      }
      logger.LogWarning("No se encontró configuración de zona horaria; usando UTC.");
      return TimeZoneInfo.Utc;
    }

    try
    {
      var tz = TZConvert.GetTimeZoneInfo(id);
      logger.LogInformation("Zona horaria resuelta: {Id}", tz.Id);
      return tz;
    }
    catch (Exception ex)
    {
      if (enforce)
      {
        throw new TimeZoneConfigurationException($"ID de zona horaria inválido o no resolvible: '{id}'.", configuredId: id, inner: ex);
      }
      logger.LogWarning(ex, "ID de zona horaria inválido o no resolvible: '{Id}'. Usando UTC.", id);
      return TimeZoneInfo.Utc;
    }
  }

  private static string? FirstNonEmpty(params string?[] values) => values.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
}
