using System;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Infrastructure.Configuration.Options.Logging;

public sealed class SysLogOptions
{
  /// <summary>Habilita/deshabilita la persistencia a dbo.SysLog.</summary>
  public bool Enabled { get; init; } = true;

  /// <summary>Para futuro (no usado en este paso). Valores: debug|info|warning|error.</summary>
  public string MinLevel { get; init; } = LoggingConstants.Levels.Error;

  /// <summary>Si true, se persiste un log Info al final de cada request HTTP.</summary>
  public bool SaveInfo { get; init; } = false;

  /// <summary>Nivel al que se registran los Problem Details 4xx. Valores: debug|info|warning|error.</summary>
  public string Problem4xxAs { get; init; } = LoggingConstants.Levels.Warning;

  public static SysLogOptions From(IConfiguration cfg)
    => new SysLogOptions
    {
      Enabled = TryParseBool(cfg[ConfigurationKeys.LoggingDB.Enabled], true),
      MinLevel = cfg[ConfigurationKeys.LoggingDB.MinLevel] ?? LoggingConstants.Levels.Error,
      SaveInfo = TryParseBool(cfg[ConfigurationKeys.LoggingDB.SaveInfo], false),
      Problem4xxAs = cfg[ConfigurationKeys.LoggingDB.Problem4xxAs] ?? LoggingConstants.Levels.Warning
    };

  private static bool TryParseBool(string? s, bool def)
    => bool.TryParse(s, out var b) ? b : def;

  public static SysLogLevel MapLevel(string? value, SysLogLevel fallback)
    => value?.ToLowerInvariant() switch
    {
      LoggingConstants.Levels.Debug => SysLogLevel.Debug,
      LoggingConstants.Levels.Info => SysLogLevel.Info,
      LoggingConstants.Levels.Warning => SysLogLevel.Warning,
      LoggingConstants.Levels.Error => SysLogLevel.Error,
      _ => fallback
    };
}
