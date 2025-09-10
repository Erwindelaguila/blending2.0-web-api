namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Auth
  {
    // Sección base para scopes dinámicos: Auth:Allow:{Scope} = "guid1,guid2,..."
    public const string AllowSection = "Auth:Allow";
    public const string DevBypass = "Auth:DevBypass"; // true/false (solo dev)
    public const string DevGroups = "Auth:DevGroups"; // CSV de grupos simulados en dev
  }
}
