using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace Function.Blending.Core.Functions.Support.Security;

public interface IPrincipalResolver
{
  Task<ClaimsPrincipal?> ResolveAsync(FunctionContext ctx, HttpRequestData req);
}
