using Function.Blending.Core.Functions.Support.Authorization;
using Function.Blending.Core.Functions.Support.Security;
using Function.Blending.Core.Shared.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Function.Blending.Core.Functions.Pipeline;

public sealed class PrincipalResolutionMiddleware : IFunctionsWorkerMiddleware
{
  private readonly IFunctionAttributeReader _attrReader;
  private readonly IPrincipalResolver _resolver;

  public PrincipalResolutionMiddleware(IFunctionAttributeReader attrReader, IPrincipalResolver resolver) => (_attrReader, _resolver) = (attrReader, resolver);

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var req = await context.GetHttpRequestDataAsync();
    if (req is null) { await next(context); return; }

    // Marcar si es webhook (para que DevBypass lo ignore)
    var isWebhook = _attrReader.Get<WebhookAttribute>(context) is not null;
    context.Items[MiscellaneousConstants.IsWebhook] = isWebhook;

    // AllowAnonymous: salta resolución (no exige auth)
    var allowAnon = _attrReader.Get<AllowAnonymousAttribute>(context) is not null;
    Console.WriteLine($"[DEBUG MIDDLEWARE] AllowAnonymous: {allowAnon}");
    
    if (!allowAnon)
    {
      Console.WriteLine("[DEBUG MIDDLEWARE] Calling PrincipalResolver.ResolveAsync");
      var principal = await _resolver.ResolveAsync(context, req);
      Console.WriteLine($"[DEBUG MIDDLEWARE] PrincipalResolver returned: {(principal != null ? "ClaimsPrincipal with " + principal.Claims.Count() + " claims" : "null")}");
      
      if (principal is not null)
      {
        Console.WriteLine("[DEBUG MIDDLEWARE] Storing principal in context.Items");
        context.Items[MiscellaneousConstants.Principal] = principal;
        
        // Log groups for verification
        var groups = principal.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToArray();
        Console.WriteLine($"[DEBUG MIDDLEWARE] Principal groups: [{string.Join(", ", groups)}]");
      }
      else
      {
        Console.WriteLine("[DEBUG MIDDLEWARE] Principal is null, not storing in context");
      }
    }

    await next(context);
  }
}
