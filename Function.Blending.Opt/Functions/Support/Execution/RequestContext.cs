using System.Security.Claims;
using Function.Blending.Opt.Functions.Support.Extensions;
using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Opt.Functions.Support.Execution;

public interface IRequestContext
{
  string? CorrelationId { get; }
  string? TraceParent { get; }
  ClaimsPrincipal? User { get; }
  IEnumerable<string> GroupIds { get; }
  string? Username { get; }
}

public sealed class RequestContext(IFunctionContextAccessor accessor) : IRequestContext
{
  private FunctionContext? Ctx => accessor.Current;

  public string? CorrelationId => Ctx?.GetCorrelationId();
  public string? TraceParent => Ctx?.GetTraceParent();
  public ClaimsPrincipal? User => Ctx?.GetUser();
  public IEnumerable<string> GroupIds => Ctx?.GetUserGroupIds() ?? Enumerable.Empty<string>();
  public string? Username => Ctx?.GetUsername();
}
