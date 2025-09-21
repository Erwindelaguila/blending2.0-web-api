using System.Security.Claims;

namespace Function.Blending.Upload.Functions.Support.Authorization;

public interface IAuthorizationService
{
  bool IsAuthorized(ClaimsPrincipal? user, string scope);
  bool IsAuthorizedForAny(ClaimsPrincipal? user, params string[] scopes);
}