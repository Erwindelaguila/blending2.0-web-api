using System;
using Function.Blending.Opt.Domain.Abstractions.Services;

namespace Function.Blending.Opt.Shared.Time;

/// <summary>
/// Implementación del reloj basada en el sistema.
/// </summary>
public sealed class SystemClock : IClock
{
  public DateTime UtcNow => DateTime.UtcNow;
  public DateTime Now => DateTime.Now;
  public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
}
