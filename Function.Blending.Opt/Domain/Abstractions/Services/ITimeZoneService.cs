using System;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ITimeZoneService
{
  /// Convierte un DateTimeOffset UTC a la TZ configurada.
  DateTimeOffset ToLocal(DateTimeOffset utc);

  /// Devuelve el Id vigente (útil para diagnosticar).
  string CurrentTimeZoneId { get; }
}
