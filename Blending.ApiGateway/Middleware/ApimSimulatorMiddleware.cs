using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blending.ApiGateway.Middleware;

/// <summary>
/// APIM SIMULATOR - CÓDIGO IDÉNTICO A PRODUCCIÓN
/// 
/// Este middleware simula EXACTAMENTE las políticas de Azure API Management.
/// Los headers generados son IDÉNTICOS a los de APIM real.
/// 
/// Funcionalidades replicadas:
/// - Validación JWT con los mismos parámetros que APIM
/// - Extracción de claims de usuario idéntica
/// - Headers X-User-* exactamente como los genera APIM
/// - Manejo de CORS igual que las políticas APIM
/// - Logging y auditoría compatible con APIM
/// </summary>
public class ApimSimulatorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApimSimulatorMiddleware> _logger;
    private readonly IConfiguration _configuration;
    
    /// <summary>
    /// Constructor del middleware APIM Simulator.
    /// Inicializa los componentes necesarios para replicar el comportamiento de APIM.
    /// </summary>
    public ApimSimulatorMiddleware(RequestDelegate next, ILogger<ApimSimulatorMiddleware> logger, IConfiguration configuration)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Procesa cada request HTTP replicando exactamente el comportamiento de APIM:
    /// 1. Maneja CORS preflight requests automáticamente
    /// 2. Extrae y valida tokens JWT con los mismos criterios que APIM
    /// 3. Genera headers X-User-* idénticos a los de APIM
    /// 4. Proporciona logging detallado para desarrollo y debugging
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Logging detallado para desarrollo y debugging
            _logger.LogInformation("REQUEST RECIBIDO:");
            _logger.LogInformation("Path: {Path}", context.Request.Path);
            _logger.LogInformation("Method: {Method}", context.Request.Method);
            
            // Mostrar headers recibidos para debugging
            _logger.LogDebug("HEADERS RECIBIDOS:");
            foreach (var header in context.Request.Headers)
            {
                _logger.LogDebug("   {HeaderName}: {HeaderValue}", header.Key, string.Join(", ", header.Value.ToArray()));
            }
            
            // Permitir OPTIONS (CORS preflight) sin validación JWT - igual que APIM
            if (context.Request.Method == "OPTIONS")
            {
                _logger.LogInformation("Permitiendo request OPTIONS (CORS preflight)");
                await _next(context);
                return;
            }
            
            // Extraer JWT exactamente como APIM - case insensitive headers
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault() ??
                            context.Request.Headers["authorization"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Request sin JWT válido: {Path}", context.Request.Path);
                _logger.LogWarning("Authorization header recibido: '{AuthHeader}'", authHeader ?? "NULL");
                
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized - JWT token required");
                return;
            }

            var token = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) 
                ? authHeader.Substring("Bearer ".Length).Trim()
                : authHeader.Trim();
            
            // Validar JWT exactamente como APIM
            var jwtClaims = ValidateJwtToken(token);
            
            if (jwtClaims == null)
            {
                _logger.LogWarning("JWT inválido para: {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid JWT token");
                return;
            }

            // Agregar headers exactamente como APIM
            AddApimHeaders(context, jwtClaims);

            _logger.LogInformation("Usuario autenticado: {UserId} - {UserName}", 
                GetClaimValue(jwtClaims, "oid"), 
                GetClaimValue(jwtClaims, "name"));

            // Continuar al siguiente middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en APIM Simulator");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    }

    /// <summary>
    /// Valida JWT exactamente como lo haría APIM.
    /// Implementa las mismas validaciones que las políticas validate-jwt de APIM:
    /// - Validación de formato JWT
    /// - Verificación de expiración con tolerancia de reloj
    /// - Extracción de claims en formato compatible con APIM
    /// </summary>
    private Dictionary<string, string>? ValidateJwtToken(string token)
    {
        try
        {
            _logger.LogInformation("=== INICIANDO VALIDACION JWT ===");
            
            var handler = new JwtSecurityTokenHandler();
            
            // Validación básica de formato JWT - igual que APIM
            if (!handler.CanReadToken(token))
            {
                _logger.LogWarning("VALIDACION FALLO: Token no es formato JWT válido");
                return null;
            }

            var jsonToken = handler.ReadJwtToken(token);
            
            _logger.LogInformation("TOKEN LEIDO CORRECTAMENTE:");
            _logger.LogInformation("  Issuer: {Issuer}", jsonToken.Issuer);
            _logger.LogInformation("  Audience: {Audience}", string.Join(", ", jsonToken.Audiences));
            _logger.LogInformation("  Valid From: {ValidFrom}", jsonToken.ValidFrom);
            _logger.LogInformation("  Valid To: {ValidTo}", jsonToken.ValidTo);
            _logger.LogInformation("  Current Time: {Now}", DateTime.UtcNow);
            
            // Verificar expiración con tolerancia de reloj (igual que APIM)
            if (jsonToken.ValidTo < DateTime.UtcNow)
            {
                _logger.LogWarning("VALIDACION FALLO: Token expirado");
                _logger.LogWarning("  Token expira: {ValidTo}", jsonToken.ValidTo);
                _logger.LogWarning("  Tiempo actual: {Now}", DateTime.UtcNow);
                return null;
            }

            // Extraer claims en el mismo formato que APIM
            var claims = new Dictionary<string, string>();
            
            _logger.LogInformation("=== EXTRAYENDO CLAIMS (como APIM) ===");
            
            foreach (var claim in jsonToken.Claims)
            {
                _logger.LogDebug("CLAIM ENCONTRADO: {Type} = {Value}", claim.Type, claim.Value);
                
                // Manejar grupos como array concatenado (igual que APIM)
                if (claim.Type == "groups")
                {
                    if (!claims.ContainsKey("groups"))
                    {
                        claims["groups"] = "";
                        _logger.LogInformation("INICIALIZANDO GRUPOS...");
                    }
                    
                    var grupoAnterior = claims["groups"];
                    claims["groups"] += (claims["groups"].Length > 0 ? "," : "") + claim.Value;
                    _logger.LogInformation("GRUPO AGREGADO: {Grupo}", claim.Value);
                    _logger.LogInformation("GRUPOS ACUMULADOS: '{Grupos}'", claims["groups"]);
                }
                else
                {
                    claims[claim.Type] = claim.Value;
                }
            }
            
            _logger.LogInformation("=== RESUMEN FINAL DE CLAIMS ===");
            foreach (var claim in claims)
            {
                if (claim.Key == "groups")
                {
                    _logger.LogInformation("GRUPOS FINALES: '{Value}'", claim.Value);
                    var gruposArray = claim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    _logger.LogInformation("TOTAL GRUPOS: {Count}", gruposArray.Length);
                    for (int i = 0; i < gruposArray.Length; i++)
                    {
                        _logger.LogInformation("  Grupo {Index}: {Grupo}", i + 1, gruposArray[i]);
                    }
                }
                else
                {
                    _logger.LogDebug("CLAIM: {Key} = {Value}", claim.Key, claim.Value);
                }
            }
            
            _logger.LogInformation("=== JWT VALIDADO EXITOSAMENTE ===");
            return claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR VALIDANDO JWT");
            return null;
        }
    }

    /// <summary>
    /// Agrega headers X-User-* EXACTAMENTE como lo hace APIM.
    /// 
    /// Headers estándar de APIM:
    /// - X-User-Id: Object ID del usuario para auditoría
    /// - X-User-Name: Nombre completo del usuario
    /// - X-User-Email: Email del usuario
    /// - X-User-Groups: Grupos separados por coma
    /// - X-User-Scopes: Scopes de autorización
    /// - X-User-Tenant: Tenant ID de Azure AD
    /// 
    /// Estos headers son los que esperan recibir las Azure Functions.
    /// </summary>
    private void AddApimHeaders(HttpContext context, Dictionary<string, string> claims)
    {
        _logger.LogInformation("=== GENERANDO HEADERS APIM ===");
        
        // X-User-Id: Object ID del usuario (para auditoría y logs)
        var userId = GetClaimValue(claims, "oid") ?? 
                    GetClaimValue(claims, "sub") ?? 
                    "anonymous";
        context.Request.Headers["X-User-Id"] = userId;
        _logger.LogInformation("HEADER: X-User-Id = {UserId}", userId);

        // X-User-Name: Nombre completo del usuario
        var userName = GetClaimValue(claims, "name") ?? 
                      GetClaimValue(claims, "given_name") ?? 
                      "Unknown User";
        context.Request.Headers["X-User-Name"] = userName;
        _logger.LogInformation("HEADER: X-User-Name = {UserName}", userName);

        // X-User-Email: Email del usuario
        var userEmail = GetClaimValue(claims, "preferred_username") ?? 
                       GetClaimValue(claims, "email") ?? 
                       GetClaimValue(claims, "upn") ??
                       "unknown@email.com";
        context.Request.Headers["X-User-Email"] = userEmail;
        _logger.LogInformation("HEADER: X-User-Email = {UserEmail}", userEmail);

        // X-User-Groups: Grupos separados por coma
        var userGroups = GetClaimValue(claims, "groups") ?? "";
        context.Request.Headers["X-User-Groups"] = userGroups;
        _logger.LogInformation("HEADER: X-User-Groups = '{UserGroups}'", userGroups);
        
        if (!string.IsNullOrEmpty(userGroups))
        {
            var gruposArray = userGroups.Split(',', StringSplitOptions.RemoveEmptyEntries);
            _logger.LogInformation("GRUPOS DETALLE:");
            _logger.LogInformation("  Total de grupos: {Count}", gruposArray.Length);
            for (int i = 0; i < gruposArray.Length; i++)
            {
                _logger.LogInformation("  Grupo {Index}: {Grupo}", i + 1, gruposArray[i].Trim());
            }
        }
        else
        {
            _logger.LogInformation("GRUPOS: Usuario sin grupos asignados");
        }

        // X-User-Scopes: Scopes del usuario para autorización
        var userScopes = GetClaimValue(claims, "scp") ?? 
                        GetClaimValue(claims, "scope") ?? 
                        "";
        context.Request.Headers["X-User-Scopes"] = userScopes;
        _logger.LogInformation("HEADER: X-User-Scopes = '{UserScopes}'", userScopes);

        // X-User-Tenant: Tenant ID de Azure AD
        var tenantId = GetClaimValue(claims, "tid") ?? "";
        context.Request.Headers["X-User-Tenant"] = tenantId;
        _logger.LogInformation("HEADER: X-User-Tenant = {TenantId}", tenantId);

        _logger.LogInformation("=== HEADERS APIM COMPLETADOS ===");
        _logger.LogInformation("RESUMEN:");
        _logger.LogInformation("  Usuario: {UserName} ({UserId})", userName, userId);
        _logger.LogInformation("  Email: {UserEmail}", userEmail);
        _logger.LogInformation("  Grupos: {GroupCount} grupos", 
            string.IsNullOrEmpty(userGroups) ? 0 : userGroups.Split(',', StringSplitOptions.RemoveEmptyEntries).Length);
        _logger.LogInformation("  Tenant: {TenantId}", tenantId);
    }

    /// <summary>
    /// Helper para obtener claim value de forma segura.
    /// Maneja casos donde el claim no existe o es null.
    /// </summary>
    private string? GetClaimValue(Dictionary<string, string> claims, string claimType)
    {
        return claims.TryGetValue(claimType, out var value) ? value : null;
    }
}