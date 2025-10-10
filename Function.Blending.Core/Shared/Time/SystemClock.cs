using System;

namespace Function.Blending.Core.Shared.Time;

/// <summary>
/// Implementación del reloj basada en el sistema - simplificada para JWT auth.
/// </summary>
public sealed class SystemClock
{
  public DateTime UtcNow => DateTime.UtcNow;
  public DateTime Now => DateTime.Now;
  public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
}
