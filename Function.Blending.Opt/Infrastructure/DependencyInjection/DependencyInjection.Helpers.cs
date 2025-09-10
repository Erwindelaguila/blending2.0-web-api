using Microsoft.Extensions.Configuration;

namespace Function.Blending.Opt.Infrastructure;

public static partial class DependencyInjection
{
  private static bool GetBool(IConfiguration cfg, string key, bool def)
    => bool.TryParse(cfg[key], out var b) ? b : def;

  private static int GetInt(IConfiguration cfg, string key, int def)
    => int.TryParse(cfg[key], out var v) ? v : def;
}
