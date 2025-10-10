namespace Function.Blending.Upload.Functions.Configuration.Options;

public sealed class AuthorizationOptions
{
  public Dictionary<string, string[]> Allow { get; } = new(StringComparer.OrdinalIgnoreCase);
  public bool DevBypass { get; set; }
  public string[] DevGroups { get; set; } = [];
}
