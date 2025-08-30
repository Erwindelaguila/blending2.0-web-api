using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Infrastructure.Services;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Common.Helpers;

/// <summary>
/// Helper para configurar headers de autorización desde Behaviors
/// Mantiene la compatibilidad con Azure Functions
/// </summary>
public static class AuthorizationContextHelper
{
    /// <summary>
    /// Configura los headers de autorización para el AuthorizationService
    /// desde el contexto de un BaseCommand
    /// </summary>
    public static void SetupAuthorizationFromCommand(
        object requestContext, 
        IAuthorizationService authorizationService)
    {
        if (requestContext is HttpRequestData req)
        {
            SetupAuthorizationHeaders(req, authorizationService);
        }
    }

    /// <summary>
    /// Configura headers de autorización para Azure Functions
    /// Extrae X-User-Groups del request y los configura en el AuthorizationService
    /// </summary>
    private static void SetupAuthorizationHeaders(HttpRequestData req, IAuthorizationService authorizationService)
    {
        var headers = new Dictionary<string, string>();

        // Extraer X-User-Id (CRÍTICO: User ID proporcionado por APIM/API Gateway)
        if (req.Headers.TryGetValues("X-User-Id", out var userId))
        {
            headers["X-User-Id"] = userId.FirstOrDefault() ?? "";
        }

        // Extraer X-User-Scopes (CRÍTICO: scopes generados por APIM/API Gateway)
        if (req.Headers.TryGetValues("X-User-Scopes", out var userScopes))
        {
            headers["X-User-Scopes"] = userScopes.FirstOrDefault() ?? "";
        }

        // Extraer X-User-Groups (proporcionado por APIM/API Gateway)
        if (req.Headers.TryGetValues("X-User-Groups", out var userGroups))
        {
            headers["X-User-Groups"] = userGroups.FirstOrDefault() ?? "";
        }

        // Extraer X-User-Name si está disponible
        if (req.Headers.TryGetValues("X-User-Name", out var userName))
        {
            headers["X-User-Name"] = userName.FirstOrDefault() ?? "";
        }

        // Extraer X-User-Email si está disponible  
        if (req.Headers.TryGetValues("X-User-Email", out var userEmail))
        {
            headers["X-User-Email"] = userEmail.FirstOrDefault() ?? "";
        }

        // Configurar headers en el servicio de autorización (método estático)
        AuthorizationService.SetCurrentRequestHeaders(headers);
    }
}
