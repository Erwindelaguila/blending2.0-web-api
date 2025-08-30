using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Auth.Application.Interfaces.Services;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Core.Infrastructure.Services;

/// <summary>
/// Servicio de usuario actual que reutiliza servicios de autenticación de Function.Blending.Auth
/// Siguiendo principios DRY y mejores prácticas
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IAuthorizationHeaderExtractor _authHeaderExtractor;
    private readonly ITokenClaimExtractor _tokenClaimExtractor;
    private readonly ITokenClaimValidator _tokenValidator;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(
        IAuthorizationHeaderExtractor authHeaderExtractor,
        ITokenClaimExtractor tokenClaimExtractor,
        ITokenClaimValidator tokenValidator,
        ILogger<CurrentUserService> logger)
    {
        _authHeaderExtractor = authHeaderExtractor ?? throw new ArgumentNullException(nameof(authHeaderExtractor));
        _tokenClaimExtractor = tokenClaimExtractor ?? throw new ArgumentNullException(nameof(tokenClaimExtractor));
        _tokenValidator = tokenValidator ?? throw new ArgumentNullException(nameof(tokenValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Guid GetCurrentUserId(object request)
    {
        try
        {
            if (request is not HttpRequestData httpRequest)
            {
                _logger.LogError("Request no es del tipo HttpRequestData");
                throw new UnauthorizedAccessException("Request type not supported");
            }

            var token = _authHeaderExtractor.ExtractJwtToken(httpRequest);
            
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Authorization token not found in request");
                throw new UnauthorizedAccessException("Authorization token not found in request");
            }

            // Validar token usando servicios de Auth
            var jwtToken = _tokenClaimExtractor.ReadJwt(token);
            if (jwtToken == null || !_tokenValidator.IsNotExpired(jwtToken))
            {
                _logger.LogWarning("JWT token is expired or invalid");
                throw new UnauthorizedAccessException("JWT token is expired or invalid");
            }

            // Extraer ObjectId usando servicios de Auth
            var userObjectId = _tokenClaimExtractor.GetUserObjectId(token);
            
            if (string.IsNullOrEmpty(userObjectId) || !Guid.TryParse(userObjectId, out var userId))
            {
                var allClaims = jwtToken?.Claims?.Select(c => $"{c.Type}: {c.Value}").ToList() ?? new List<string>();
                var claimsDebug = string.Join(", ", allClaims);
                
                _logger.LogWarning("User ID not found in JWT token. Available claims: {Claims}", claimsDebug);
                throw new UnauthorizedAccessException($"User ID not found in JWT token. Available claims: [{claimsDebug}]");
            }

            _logger.LogDebug("Successfully extracted user ID: {UserId}", userId);
            return userId;
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while extracting user ID from request");
            throw new UnauthorizedAccessException("Error processing authentication token", ex);
        }
    }

    public string? GetCurrentUserName(object request)
    {
        try
        {
            if (request is not HttpRequestData httpRequest)
            {
                return null;
            }

            var token = _authHeaderExtractor.ExtractJwtToken(httpRequest);
            
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var jwtToken = _tokenClaimExtractor.ReadJwt(token);
            if (jwtToken == null || !_tokenValidator.IsNotExpired(jwtToken))
            {
                return null;
            }

            var userName = _tokenClaimExtractor.GetUserName(token);
            
            _logger.LogDebug("Extracted user name: {UserName}", userName ?? "null");
            return userName;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error extracting user name from token");
            return null;
        }
    }

    public bool IsAuthenticated(object request)
    {
        try
        {
            if (request is not HttpRequestData httpRequest)
            {
                return false;
            }

            var token = _authHeaderExtractor.ExtractJwtToken(httpRequest);
            
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var jwtToken = _tokenClaimExtractor.ReadJwt(token);
            var isValid = jwtToken != null && _tokenValidator.IsNotExpired(jwtToken);
            
            _logger.LogDebug("Token authentication check result: {IsAuthenticated}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Token authentication failed");
            return false;
        }
    }
}
