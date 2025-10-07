namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ITimeZoneResolver
{
  /// Devuelve TimeZoneInfo a partir de un Id (IANA o Windows) o null si es inválido.
  TimeZoneInfo? TryResolve(string? timeZoneId);
}
