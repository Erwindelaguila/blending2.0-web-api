using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Upload.Functions.Support.Security;

public interface IPrincipalBuilder
{
  /// Devuelve un principal si aplica; si no aplica, null.
  Task<ClaimsPrincipal?> TryBuildAsync(FunctionContext ctx, HttpRequestData req);
}
