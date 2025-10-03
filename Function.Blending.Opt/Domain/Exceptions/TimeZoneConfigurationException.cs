namespace Function.Blending.Opt.Domain.Exceptions;

public sealed class TimeZoneConfigurationException : Exception
{
  public string? ConfiguredId { get; }

  public TimeZoneConfigurationException(string message, string? configuredId = null, Exception? inner = null)
    : base(message, inner)
  {
    ConfiguredId = configuredId;
  }
}
