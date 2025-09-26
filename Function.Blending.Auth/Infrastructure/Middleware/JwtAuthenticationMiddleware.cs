using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Function.Blending.Auth.Application.Common;
using Function.Blending.Auth.Application.Common.Wrappers;
using Function.Blending.Auth.Application.Interfaces.Services;
using Function.Blending.Auth.Application.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Infrastructure.Middleware
{
    public class JwtAuthenticationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<JwtAuthenticationMiddleware> _logger;
        private readonly IAuthorizationHeaderExtractor _authExtractor;
        private readonly ITokenService _tokenService;

        public JwtAuthenticationMiddleware(
            ILogger<JwtAuthenticationMiddleware> logger,
            IAuthorizationHeaderExtractor authExtractor,
            ITokenService tokenService)
        {
            _logger = logger;
            _authExtractor = authExtractor;
            _tokenService = tokenService;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                var requestData = await context.GetHttpRequestDataAsync();
                if (requestData != null)
                {
                    var authResult = await AuthenticateRequestAsync(requestData);
                    
                    if (!authResult.IsAuthenticated)
                    {
                        await SetUnauthorizedResponseAsync(context, authResult.ErrorMessage);
                        return;
                    }

           
                    if (authResult.Claims != null)
                    {
                        var userClaims = ExtractUserClaimsFromPrincipal(authResult.Claims);
                        context.Items["UserClaims"] = userClaims;
                    }
                    if (authResult.Token != null)
                        context.Items["JwtToken"] = authResult.Token;
                }

                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el middleware de autenticación JWT");
                await SetUnauthorizedResponseAsync(context, "Error interno de autenticación");
            }
        }

        private async Task<AuthenticationResult> AuthenticateRequestAsync(HttpRequestData request)
        {
            try
            {
                var token = _authExtractor.ExtractJwtToken(request);
                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticationResult.Failed("Token no proporcionado");
                }

                var isValid = await _tokenService.ValidateTokenAsync(token);
                if (isValid)
                {
   
                    var objectId = _tokenService.GetUserObjectId(token);
                    var userName = _tokenService.GetUserName(token);
                    var groups = _tokenService.GetUserGroups(token);

                    var claims = new List<Claim>();
                    if (!string.IsNullOrEmpty(objectId))
                        claims.Add(new Claim(JwtClaimTypes.Oid, objectId));
                    if (!string.IsNullOrEmpty(userName))
                        claims.Add(new Claim(JwtClaimTypes.Name, userName));
                    if (groups != null)
                    {
                        foreach (var group in groups)
                            claims.Add(new Claim(JwtClaimTypes.Groups, group));
                    }

                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                    return AuthenticationResult.Success(claimsPrincipal, token);
                }

                return AuthenticationResult.Failed("Token inválido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando token");
                return AuthenticationResult.Failed("Error validando token");
            }
        }

        private async Task SetUnauthorizedResponseAsync(FunctionContext context, string? message)
        {
            var requestData = await context.GetHttpRequestDataAsync();
            var response = requestData!.CreateResponse(HttpStatusCode.Unauthorized);

            response.Headers.Add("Content-Type", "application/json");
            
            var errorResponse = BaseResponse<object>.Fail(message ?? "Token inválido o no autorizado", 401);

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await response.WriteStringAsync(jsonResponse);
            
            context.GetInvocationResult().Value = response;
        }

        private UserClaims ExtractUserClaimsFromPrincipal(ClaimsPrincipal principal)
        {
            return new UserClaims
            {
                ObjectId = principal.FindFirst(JwtClaimTypes.Oid)?.Value,
                Name = principal.FindFirst(JwtClaimTypes.Name)?.Value,
                Groups = principal.FindAll(JwtClaimTypes.Groups).Select(c => c.Value).ToList()
            };
        }
    }

    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; }
        public ClaimsPrincipal? Claims { get; }
        public string? Token { get; }
        public string? ErrorMessage { get; }

        private AuthenticationResult(bool isAuthenticated, ClaimsPrincipal? claims, string? token, string? errorMessage)
        {
            IsAuthenticated = isAuthenticated;
            Claims = claims;
            Token = token;
            ErrorMessage = errorMessage;
        }

        public static AuthenticationResult Success(ClaimsPrincipal claims, string token)
            => new(true, claims, token, null);

        public static AuthenticationResult Failed(string errorMessage)
            => new(false, null, null, errorMessage);
    }
}