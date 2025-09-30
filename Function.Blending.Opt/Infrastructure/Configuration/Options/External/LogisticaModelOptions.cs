namespace Function.Blending.Opt.Infrastructure.Configuration.Options.External;

public sealed class LogisticaModelOptions
{
  public string BaseUrl { get; set; } = string.Empty;
  public string StartPath { get; set; } = "/api/logistics/start";
  public int TimeoutSeconds { get; set; } = 30; // usa 30s por tu requerimiento de ACK
  public string? ApiKey { get; set; }
}
