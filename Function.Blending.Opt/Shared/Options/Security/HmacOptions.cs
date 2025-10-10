namespace Function.Blending.Opt.Shared.Options.Security;

public sealed class HmacOptions
{
  public string Resolver { get; set; } = "KeyVault";
  public string VaultUrl { get; set; } = string.Empty;
  public int CacheSeconds { get; set; } = 600;

  // Opcional, útil en multi-tenant:
  public string? TenantId { get; set; }

  // Controlamos cómo autenticamos al SecretClient
  public HmacCredentialOptions Credential { get; set; } = new();
  public HmacFallbackOptions Fallback { get; set; } = new();
}

public sealed class HmacCredentialOptions
{
  /// <summary>
  /// "ManagedIdentity" | "Environment" | "Interactive" | "DefaultNoCli" | "Default"
  /// </summary>
  public string Mode { get; set; } = "Default";

  /// <summary>Para MI asignada por usuario o SP (ClientId).</summary>
  public string? ClientId { get; set; }

  /// <summary>Fuerza tenant cuando aplica.</summary>
  public string? TenantId { get; set; }
}

public sealed class HmacFallbackOptions
{
  /// <summary>
  public bool Enable { get; set; }

  public string? AllowList { get; set; }

  public string? SecretsPrefix { get; set; }
}
