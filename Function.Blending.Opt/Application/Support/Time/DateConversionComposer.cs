using Function.Blending.Opt.Application.Support.Meta;
using Function.Blending.Opt.Domain.Abstractions.Services;

namespace Function.Blending.Opt.Application.Support.Time;

public static class DateConversionComposer
{
  /// <summary>
  /// Aplica (si corresponde) la conversión UTC→Local sobre el grafo <typeparamref name="T"/> y
  /// devuelve el mismo objeto envuelto con metadatos sobre la conversión.
  /// </summary>
  public static WithMeta<T, DateConversionMeta> Wrap<T>(
    T data,
    bool convertDates,
    string? tzId,
    ITimeZoneService tzService,
    ITimeZoneResolver tzResolver)
  {
    TimeZoneInfo? tzOverride = convertDates ? tzResolver.TryResolve(tzId) : null;

    if (convertDates && data is not null)
      DateTimeGraphLocalizer.ConvertUtcToLocal(data!, tzService, tzOverride);

    var effectiveTz = (tzOverride ?? TimeZoneInfo.FindSystemTimeZoneById(tzService.CurrentTimeZoneId)).Id;

    var meta = new DateConversionMeta(
      Converted: convertDates,
      EffectiveTimeZoneId: effectiveTz,
      OverrideApplied: tzOverride is not null
    );

    return new WithMeta<T, DateConversionMeta>(data, meta);
  }
}
