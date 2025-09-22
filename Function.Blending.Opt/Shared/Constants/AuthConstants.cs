namespace Function.Blending.Opt.Shared.Constants;

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
      public const string Strict = "Strict";
    }
    public static class Types
    {
      public const string Relaxed = $"{Name}-{Modes.Relaxed}";
      public const string Strict = $"{Name}-{Modes.Strict}";
    }
    public static class Tags
    {
      public static readonly string Relaxed = Modes.Relaxed.ToLower();
      public static readonly string Strict = Modes.Strict.ToLower();
    }
  }

  public static class DevBypassAuth
  {
    public const string Name = "DevBypass";

    public const string Dev = "dev";
  }

  public static class HmacAuth
  {
    public const string Name = "Hmac";

    public const string ItemsKey = "HmacValid";
    public const string HmacGroupsPrefix = "Auth_Hmac_Groups_";
  }

  public static class EasyAuth
  {
    public const string Name = "EasyAuth";

    public const string HeaderKey = "X-MS-CLIENT-PRINCIPAL";
  }

  public static class LocalHeaderAuth
  {
    public const string Name = "LocalHeader";

    public const string HeaderKey = "X-LOCAL-GROUPS";

  }
}
