using System;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

/// <summary>
/// Reloj abstracto para testear tiempos determinísticamente.
/// </summary>
public interface IClock
{
  DateTime UtcNow { get; }
  DateTime Now { get; }
  DateTimeOffset UtcNowOffset { get; }
}
