using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace Function.Blending.Opt.Functions.Support.Security;

public sealed class PrincipalResolver : IPrincipalResolver
{
  private readonly IEnumerable<IPrincipalBuilder> _builders;
  public PrincipalResolver(IEnumerable<IPrincipalBuilder> builders) => _builders = builders;

  public async Task<ClaimsPrincipal?> ResolveAsync(FunctionContext ctx, HttpRequestData req)
  {
    foreach (var b in _builders)
    {
      var p = await b.TryBuildAsync(ctx, req);
      if (p is not null) return p;
    }
    return null;
  }
}
