using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace Function.Blending.Opt.Functions.Support.Security;

public interface IPrincipalBuilder
{
  /// Devuelve un principal si aplica; si no aplica, null.
  Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, HttpRequestData req);
}
