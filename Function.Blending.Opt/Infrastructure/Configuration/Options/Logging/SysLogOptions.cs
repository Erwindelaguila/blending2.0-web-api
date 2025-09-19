using System;
using Function.Blending.Opt.Domain.Logging;
using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Infrastructure.Configuration.Options.Logging;

public sealed class SysLogOptions
{
  /// <summary>Habilita/deshabilita la persistencia a dbo.SysLog.</summary>
  public bool Enabled { get; init; } = true;

  /// <summary>Para futuro (no usado en este paso). Valores: debug|info|warning|error.</summary>
  public string MinLevel { get; init; } = "error";

  /// <summary>Si true, se persiste un log Info al final de cada request HTTP.</summary>
  public bool SaveInfo { get; init; } = false;

  /// <summary>Nivel al que se registran los Problem Details 4xx. Valores: debug|info|warning|error.</summary>
  public string Problem4xxAs { get; init; } = "warning";

  public static SysLogOptions From(IConfiguration cfg)
    => new SysLogOptions
    {
      Enabled = TryParseBool(cfg["Logging_Db_Enabled"], true),
      MinLevel = cfg["Logging_Db_MinLevel"] ?? "error",
      SaveInfo = TryParseBool(cfg["Logging_Db_SaveInfo"], false),
      Problem4xxAs = cfg["Logging_Db_Problem4xxAs"] ?? "warning"
    };

  private static bool TryParseBool(string? s, bool def)
    => bool.TryParse(s, out var b) ? b : def;

  public static SysLogLevel MapLevel(string? value, SysLogLevel fallback)
    => value?.ToLowerInvariant() switch
    {
      "debug" => SysLogLevel.Debug,
      "info" => SysLogLevel.Info,
      "warning" => SysLogLevel.Warning,
      "error" => SysLogLevel.Error,
      _ => fallback
    };
}
