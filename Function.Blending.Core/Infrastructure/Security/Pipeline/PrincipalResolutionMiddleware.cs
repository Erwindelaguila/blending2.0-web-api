using Function.Blending.Core.Infrastructure.Security.Shared.Constants;
using Function.Blending.Core.Infrastructure.Security.Support.Authorization;
using Function.Blending.Core.Infrastructure.Security.Support.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Function.Blending.Core.Infrastructure.Security.Pipeline;

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


    // AllowAnonymous: salta resolución (no exige auth)
    var allowAnon = _attrReader.Get<AllowAnonymousAttribute>(context) is not null;
    if (!allowAnon)
    {
      var principal = await _resolver.ResolveAsync(context, req);
      if (principal is not null)
        context.Items[MiscellaneousConstants.Principal] = principal;
    }

    await next(context);
  }
}
