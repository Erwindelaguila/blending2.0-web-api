using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Function.Blending.Core.Functions.Configuration.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Function.Blending.Core.Functions.Support.Security;

public sealed class PrincipalResolver : IPrincipalResolver
{
  private readonly AuthorizationOptions _options;
  
  public PrincipalResolver(IOptions<AuthorizationOptions> options) 
  {
    _options = options.Value;
  }

  public Task<ClaimsPrincipal?> ResolveAsync(FunctionContext ctx, HttpRequestData req)
  {
    Console.WriteLine("[DEBUG RESOLVER] Starting PrincipalResolver.ResolveAsync");
    
    // DevBypass - for development/testing
    if (_options.DevBypass && req.Headers.Contains("X-Dev-User"))
    {
      var devUser = req.Headers.GetValues("X-Dev-User").FirstOrDefault();
      if (!string.IsNullOrEmpty(devUser))
      {
        Console.WriteLine($"[DEBUG RESOLVER] DevBypass activated for user: {devUser}");
        return Task.FromResult<ClaimsPrincipal?>(new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
          new Claim(ClaimTypes.Name, devUser),
          new Claim("scope", "Administrador") // DevBypass gets admin access
        }, "DevBypass")));
      }
    }

    // Bearer Token authentication
    Console.WriteLine("[DEBUG RESOLVER] Checking for Authorization header");
    if (req.Headers.TryGetValues("Authorization", out var authHeaders))
    {
      Console.WriteLine($"[DEBUG RESOLVER] Found Authorization header with {authHeaders.Count()} values");
      var authHeader = authHeaders.FirstOrDefault();
      Console.WriteLine($"[DEBUG RESOLVER] Authorization header: {authHeader?.Substring(0, Math.Min(20, authHeader?.Length ?? 0))}...");
      
      if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
      {
        var token = authHeader["Bearer ".Length..].Trim();
        Console.WriteLine($"[DEBUG RESOLVER] Extracted Bearer token, length: {token.Length}");
        
        if (!string.IsNullOrEmpty(token))
        {
          try
          {
            Console.WriteLine("[DEBUG RESOLVER] Attempting to parse JWT token");
            
            // Verificar que el token tiene el formato correcto (3 partes separadas por puntos)
            var parts = token.Split('.');
            Console.WriteLine($"[DEBUG RESOLVER] JWT has {parts.Length} parts (should be 3)");
            
            if (parts.Length != 3)
            {
              Console.WriteLine($"[DEBUG RESOLVER] Invalid JWT format - expected 3 parts, got {parts.Length}");
              throw new ArgumentException("Invalid JWT format");
            }
            
            // Decodificar el payload manualmente (bypass del header problemático)
            try
            {
              Console.WriteLine("[DEBUG RESOLVER] Attempting manual payload decoding");
              var payload = parts[1];
              
              // Ajustar padding si es necesario para Base64
              while (payload.Length % 4 != 0)
                payload += "=";
              
              // Convertir de Base64URL a Base64 estándar
              var base64Payload = payload.Replace('-', '+').Replace('_', '/');
              var payloadBytes = Convert.FromBase64String(base64Payload);
              var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
              
              Console.WriteLine($"[DEBUG RESOLVER] Successfully decoded payload: {payloadJson.Substring(0, Math.Min(200, payloadJson.Length))}...");
              
              // Parsear el JSON del payload
              var payloadData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);
              
              // Crear claims desde el payload JSON
              var claims = new List<Claim>();
              
              foreach (var kvp in payloadData!)
              {
                if (kvp.Value is System.Text.Json.JsonElement jsonElement)
                {
                  if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                  {
                    // Handle arrays (like groups)
                    foreach (var arrayItem in jsonElement.EnumerateArray())
                    {
                      if (arrayItem.ValueKind == System.Text.Json.JsonValueKind.String)
                      {
                        claims.Add(new Claim(kvp.Key, arrayItem.GetString()!));
                      }
                    }
                  }
                  else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String)
                  {
                    claims.Add(new Claim(kvp.Key, jsonElement.GetString()!));
                  }
                  else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                  {
                    claims.Add(new Claim(kvp.Key, jsonElement.ToString()));
                  }
                }
              }
              
              Console.WriteLine($"[DEBUG RESOLVER] Created {claims.Count} claims from payload");
              
              // Log groups specifically
              var groupClaims = claims.Where(c => c.Type == "groups").ToList();
              Console.WriteLine($"[DEBUG RESOLVER] Found {groupClaims.Count} group claims: [{string.Join(", ", groupClaims.Select(c => c.Value))}]");
              
              var identity = new ClaimsIdentity(claims, "Bearer");
              var principal = new ClaimsPrincipal(identity);
              Console.WriteLine("[DEBUG RESOLVER] Successfully created ClaimsPrincipal via manual parsing");
              
              return Task.FromResult<ClaimsPrincipal?>(principal);
            }
            catch (Exception ex)
            {
              Console.WriteLine($"[DEBUG RESOLVER] Manual payload decoding failed: {ex.Message}");
              throw;
            }
          }
          catch (Exception ex)
          {
            Console.WriteLine($"[DEBUG RESOLVER] Error parsing JWT token: {ex.Message}");
            // Token is invalid, continue to return null
          }
        }
        else
        {
          Console.WriteLine("[DEBUG RESOLVER] Token is empty after extraction");
        }
      }
      else
      {
        Console.WriteLine("[DEBUG RESOLVER] Authorization header doesn't start with 'Bearer '");
      }
    }
    else
    {
      Console.WriteLine("[DEBUG RESOLVER] No Authorization header found");
    }

    Console.WriteLine("[DEBUG RESOLVER] Returning null principal");
    return Task.FromResult<ClaimsPrincipal?>(null);
  }
}
