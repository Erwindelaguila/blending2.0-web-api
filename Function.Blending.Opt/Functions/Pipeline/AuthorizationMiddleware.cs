using System.Net;
using System.Security.Claims;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.Pipeline;

public sealed class AuthorizationMiddleware(
    ILogger<AuthorizationMiddleware> logger,
    IAuthorizationService authz,
    IFunctionAttributeReader attrReader,
    IProblemDetailsWriter problem)  // ← usa el writer central
  : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var req = await context.GetHttpRequestDataAsync();
    if (req is null) { await next(context); return; }

    if (IsAllowAnonymous(context))
    {
      await next(context);
      return;
    }

    var requiredScopes = GetRequiredScopes(context);
    var principal = GetPrincipal(context);

    if (requiredScopes.Length > 0)
    {
      LogScopesAndGroups(principal, requiredScopes);

      if (principal is null || !authz.IsAuthorizedForAny(principal, requiredScopes))
      {
        await problem.WriteAndSetInvocationResultAsync(
          context,
          req,
          HttpStatusCode.Forbidden,
          type: "urn:blending:error:forbidden",
          title: "Forbidden",
          detail: "You don't have permission to access this resource.",
          extensions: new Dictionary<string, object?>
          {
            ["scopeRequired"] = requiredScopes,
            ["principalGroups"] = principal?.Claims
              .Where(c => c.Type is "groups" or "roles")
              .Select(c => c.Value)
              .ToArray()
          });
        return;
      }
    }
    else
    {
      if (!IsAuthenticated(principal))
      {
        await problem.WriteAndSetInvocationResultAsync(
          context,
          req,
          HttpStatusCode.Unauthorized,
          type: "urn:blending:error:unauthorized",
          title: "Unauthorized",
          detail: "Authentication is required to access this resource.");
        return;
      }
    }

    await next(context);
  }

  private bool IsAllowAnonymous(FunctionContext ctx)
      => attrReader.Get<AllowAnonymousAttribute>(ctx) is not null;

  private static ClaimsPrincipal? GetPrincipal(FunctionContext ctx)
      => ctx.Items.TryGetValue(MiscellaneousKeys.Principal, out var p) ? p as ClaimsPrincipal : null;

  private static bool IsAuthenticated(ClaimsPrincipal? principal)
      => principal is not null && (principal.Identity?.IsAuthenticated ?? false);

  private string[] GetRequiredScopes(FunctionContext ctx)
  {
    var attr = attrReader.Get<RequireScopesAttribute>(ctx);
    return attr?.Scopes is { Length: > 0 } scopes ? scopes : Array.Empty<string>();
  }

  private void LogScopesAndGroups(ClaimsPrincipal? principal, string[] requiredScopes)
  {
    try
    {
      var groups = principal?.Claims.Where(c => c.Type is "groups" or "roles").Select(c => c.Value).ToArray() ?? [];
      var groupsTxt = groups.Length == 0 ? "<none>" : string.Join(",", groups);
      logger.LogInformation("Auth: required=[{Scopes}] | groups=[{Groups}]", string.Join(",", requiredScopes), groupsTxt);
    }
    catch { /* logging nunca rompe el pipeline */ }
  }
}
