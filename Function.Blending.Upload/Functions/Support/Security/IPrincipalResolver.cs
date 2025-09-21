using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Upload.Functions.Support.Security;

public interface IPrincipalResolver
{
  Task<ClaimsPrincipal?> ResolveAsync(FunctionContext ctx, HttpRequestData req);
}
