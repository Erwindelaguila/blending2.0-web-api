using System.Security.Claims;

namespace Function.Blending.Core.Infrastructure.Security.Support.Authorization;

public interface IAuthorizationService
{
  bool IsAuthorized(ClaimsPrincipal? user, string scope);
  bool IsAuthorizedForAny(ClaimsPrincipal? user, params string[] scopes);
}