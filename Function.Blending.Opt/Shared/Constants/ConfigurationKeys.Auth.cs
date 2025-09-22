namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Auth
  {
    // Sección base para scopes dinámicos: Auth_Allow:{Scope} = "guid1,guid2,..."
    public const string AllowSection = "Auth_Allow";
    public const string DevBypass = "Auth_DevBypass"; // true/false (solo dev)
    public const string DevGroups = "Auth_DevGroups"; // CSV de grupos simulados en dev
    public const string EnableBearer = "Auth_EnableBearerTokens"; // true/false
    public const string EnableEasyAuth = "Auth_EnableEasyAuth"; // true/false
    public const string EnableHmacPrincipal = "Auth_EnableHmacPrincipal"; // true/false
    public const string EnableLocalHeaderPrincipal = "Auth_EnableLocalHeaderPrincipal"; // true/false

    public static partial class Bearer
    {
      public const string ValidationMode = "Auth_Bearer_ValidationMode"; // "Strict" (servicio) o "Relaxed" (solo decodifica)
      public const string TenantId = "Auth_Bearer_TenantId"; // obligatorio
      public const string Authority = "Auth_Bearer_Authority"; // opcional (si no, se construye con TenantId)
      public const string Audience = "Auth_Bearer_Audience"; // obligatorio, CSV
      public const string ValidIssuer = "Auth_Bearer_ValidIssuer"; // opcional (si no, se construye con TenantId)
      public const string ClockSkewSeconds = "Auth_Bearer_ClockSkewSeconds"; // opcional, por defecto 300 (5 min)
      public const string ValidateLifetime = "Auth_Bearer_ValidateLifetime"; // true/false, opcional, por defecto true
    }
  }
}
