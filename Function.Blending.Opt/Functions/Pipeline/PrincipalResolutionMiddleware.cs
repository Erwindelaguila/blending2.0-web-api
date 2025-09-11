using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Security;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Function.Blending.Opt.Functions.Pipeline;

public sealed class PrincipalResolutionMiddleware : IFunctionsWorkerMiddleware
{
  private readonly IFunctionAttributeReader _attrReader;
  private readonly IPrincipalResolver _resolver;

  public PrincipalResolutionMiddleware(IFunctionAttributeReader attrReader, IPrincipalResolver resolver)
      => (_attrReader, _resolver) = (attrReader, resolver);

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var req = await context.GetHttpRequestDataAsync();
    if (req is null) { await next(context); return; }

    // Marcar si es webhook (para que DevBypass lo ignore)
    var isWebhook = _attrReader.Get<WebhookAttribute>(context) is not null;
    context.Items[MiscellaneousKeys.IsWebhook] = isWebhook;

    // AllowAnonymous: salta resolución (no exige auth)
    var allowAnon = _attrReader.Get<AllowAnonymousAttribute>(context) is not null;
    if (!allowAnon)
    {
      var principal = await _resolver.ResolveAsync(context, req);
      if (principal is not null)
        context.Items[MiscellaneousKeys.Principal] = principal;
    }

    await next(context);
  }
}
