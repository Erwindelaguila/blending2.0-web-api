using System;
using System.Collections.Generic;
using System.Linq;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class TokenConfigurationService : ITokenConfigurationService
    {
        public bool IsDevelopmentMode { get; }
        public string TenantId { get; }
        public string ExpectedClientId { get; }
        public List<string> AllowedClientIds { get; }
        public string Authority { get; }

        public TokenConfigurationService()
        {
            IsDevelopmentMode = Environment.GetEnvironmentVariable("Environment") == "Development";
            
            TenantId = Environment.GetEnvironmentVariable("AzureAD_TenantId") 
                ?? throw new InvalidOperationException("La variable de entorno 'AzureAD_TenantId' es requerida");
            
            ExpectedClientId = Environment.GetEnvironmentVariable("AzureAD_ClientId") 
                ?? throw new InvalidOperationException("La variable de entorno 'AzureAD_ClientId' es requerida");
            
            var allowedClientIdsString = Environment.GetEnvironmentVariable("AzureAD_AllowedClientIds") ?? ExpectedClientId;
            AllowedClientIds = allowedClientIdsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim()).ToList();
            
            Authority = $"https://login.microsoftonline.com/{TenantId}/v2.0";
        }
    }
}
