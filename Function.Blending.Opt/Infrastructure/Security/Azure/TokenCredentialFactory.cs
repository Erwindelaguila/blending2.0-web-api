using System.Collections.Generic;
using Azure.Core;
using Azure.Identity;
using Function.Blending.Opt.Shared.Options.Security;

namespace Function.Blending.Opt.Infrastructure.Security.Azure;

public static class TokenCredentialFactory
{
  public static TokenCredential Create(HmacOptions opt)
  {
    string mode = (opt.Credential?.Mode ?? "Default").Trim().ToLowerInvariant();
    string? tenant = opt.Credential?.TenantId ?? opt.TenantId;
    string? clientId = opt.Credential?.ClientId;

    switch (mode)
    {
      case "managedidentity":
        // PROD: Azure Functions (Linux) con Identity administrada
        return string.IsNullOrWhiteSpace(clientId)
          ? new ManagedIdentityCredential()
          : new ManagedIdentityCredential(clientId);

      case "environment":
        // LOCAL/CI: Service Principal vía AZURE_* (si lo usaran)
        return new EnvironmentCredential();

      case "interactive":
        // LOCAL: abre login en navegador; no necesita VS/VSC/CLI
        return new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions
        {
          TenantId = tenant
        });

      case "defaultnocli":
        // Cadena **explícita** SIN AzureCli/VS/VSCode/SharedTokenCache (nada obsoleto).
        // Orden: ManagedIdentity -> Environment -> Interactive (fallback controlado)
        var chain = new List<TokenCredential>
        {
          string.IsNullOrWhiteSpace(clientId)
            ? new ManagedIdentityCredential()
            : new ManagedIdentityCredential(clientId),
          new EnvironmentCredential(),
          new InteractiveBrowserCredential(new InteractiveBrowserCredentialOptions { TenantId = tenant })
        };
        return new ChainedTokenCredential([.. chain]);

      case "default":
      default:
        // Cadena por defecto de Azure.Identity (sin propiedades obsoletas)
        return new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
          ExcludeInteractiveBrowserCredential = false,
          SharedTokenCacheTenantId = tenant,
          VisualStudioTenantId = tenant,
          VisualStudioCodeTenantId = tenant
        });
    }
  }
}
