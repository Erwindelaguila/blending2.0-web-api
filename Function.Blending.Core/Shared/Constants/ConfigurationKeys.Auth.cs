namespace Function.Blending.Core.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Auth
  {

    public const string AllowSection = "Auth_Allow";
    public const string DevBypass = "Auth_DevBypass"; // true/false (solo dev)
    public const string DevGroups = "Auth_DevGroups"; // CSV de grupos simulados en dev
    public const string EnableBearer = "Auth_EnableBearerTokens"; // true/false

    public static partial class Bearer
    {
      public const string ValidationMode = "Auth_Bearer_ValidationMode"; // Solo "Relaxed"
    }
  }
}
