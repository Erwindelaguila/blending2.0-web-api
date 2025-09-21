using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Infrastructure.Security.Support.Security;

public interface IPrincipalResolver
{
  Task<ClaimsPrincipal?> ResolveAsync(FunctionContext ctx, HttpRequestData req);
}
