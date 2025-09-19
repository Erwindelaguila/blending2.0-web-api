namespace Function.Blending.Opt.Application.Abstractions.External;

public sealed record DispatchResult(DispatchOutcome Outcome, int? StatusCode = null, string? Message = null)
{
  public static DispatchResult Ok(int? statusCode = 202, string? message = null) => new(DispatchOutcome.Accepted, statusCode, message);

  public static DispatchResult Fail(int? statusCode = null, string? message = null) => new(DispatchOutcome.Rejected, statusCode, message);

  public static DispatchResult Timeout(string? message = "Timeout") => new(DispatchOutcome.TimedOut, null, message);

  public static DispatchResult Error(string? message = "Error") => new(DispatchOutcome.Error, null, message);
}
