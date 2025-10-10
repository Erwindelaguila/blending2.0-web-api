using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Function.Blending.Opt.Functions.Support.Authorization;

public interface IAuthorizationService
{
  bool IsAuthorized(ClaimsPrincipal? user, string scope);
  bool IsAuthorizedForAny(ClaimsPrincipal? user, params string[] scopes);
}