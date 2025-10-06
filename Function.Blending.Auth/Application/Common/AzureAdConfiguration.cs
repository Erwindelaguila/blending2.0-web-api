using System;
using System.Collections.Generic;

namespace Function.Blending.Auth.Application.Common
{
    public class AzureAdConfiguration
    {
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string Authority => $"https://login.microsoftonline.com/{TenantId}/v2.0";
        public IEnumerable<string> ValidIssuers => new[]
        {
            $"https://login.microsoftonline.com/{TenantId}/v2.0",
            $"https://sts.windows.net/{TenantId}/"
        };
        public IEnumerable<string> ValidAudiences => new[] 
        { 
            ClientId, 
            $"api://{ClientId}" 
        };
    }
}