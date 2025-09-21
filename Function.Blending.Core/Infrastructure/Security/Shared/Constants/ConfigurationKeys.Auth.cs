namespace Function.Blending.Core.Infrastructure.Security.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Auth
  {
    // Sección base para scopes dinámicos: Auth_Allow:{Scope} = "guid1,guid2,..."
    public const string AllowSection = "Auth_Allow";
    public const string DevBypass = "Auth_DevBypass"; // true/false (solo dev)
    public const string DevGroups = "Auth_DevGroups"; // CSV de grupos simulados en dev
  }
}
