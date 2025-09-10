using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Functions.Configuration.Options;

public sealed class AuthorizationOptions
{
  public Dictionary<string, string[]> Allow { get; } = new(StringComparer.OrdinalIgnoreCase);
  public bool DevBypass { get; set; }
  public string[] DevGroups { get; set; } = [];
}
