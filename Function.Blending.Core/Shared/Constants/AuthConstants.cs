namespace Function.Blending.Core.Shared.Constants;

public static class AuthConstants
{
  public const string Authorization = "Authorization";
  public const string Groups = "groups";
  public const string AllowPrefix = "Auth_Allow_";
  public const string AuthType = "auth_type";

  public static class BearerAuth
  {
    public const string Name = "Bearer";
    public static class Modes
    {
      public const string Relaxed = "Relaxed";
    }
    public static class Types
    {
      public const string Relaxed = $"{Name}-{Modes.Relaxed}";
    }
    public static class Tags
    {
      public static readonly string Relaxed = Modes.Relaxed.ToLower();
    }
  }

  public static class DevBypassAuth
  {
    public const string Name = "DevBypass";
    public const string Dev = "dev";
  }
}
