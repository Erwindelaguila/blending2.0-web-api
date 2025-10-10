namespace Function.Blending.Opt.Domain.Logging;

public enum SysLogLevel
{
  Debug = 0,
  Info = 1,
  Warning = 2,
  Error = 3
}

public static class SysLogLevelExtensions
{
  public static string ToDbString(this SysLogLevel level)
    => level switch
    {
      SysLogLevel.Debug => "debug",
      SysLogLevel.Info => "info",
      SysLogLevel.Warning => "warning",
      _ => "error"
    };
}
