using System.Security.Claims;
using Function.Blending.Upload.Functions.Support.Extensions;
using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Upload.Functions.Support.Execution;

public interface IRequestContext
{
  string? CorrelationId { get; }
  ClaimsPrincipal? User { get; }
  IEnumerable<string> GroupIds { get; }
  string? Username { get; }
}

public sealed class RequestContext : IRequestContext
{
  private readonly IFunctionContextAccessor _accessor;
  public RequestContext(IFunctionContextAccessor accessor) => _accessor = accessor;

  private FunctionContext? Ctx => _accessor.Current;

  public string? CorrelationId => Ctx?.GetCorrelationId();
  public ClaimsPrincipal? User => Ctx?.GetUser();
  public IEnumerable<string> GroupIds => Ctx?.GetUserGroupIds() ?? Enumerable.Empty<string>();
  public string? Username => Ctx?.GetUsername();
}
