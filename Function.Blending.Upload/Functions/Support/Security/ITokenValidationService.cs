using System.Security.Claims;

namespace Function.Blending.Upload.Functions.Support.Security;

public interface ITokenValidationService
{
  /// <summary>
  /// Valida criptográficamente un JWT y devuelve un ClaimsPrincipal normalizado,
  /// o null si el token no es válido.
  /// </summary>
  Task<ClaimsPrincipal?> ValidateAndNormalizeAsync(string jwtRaw);
}
