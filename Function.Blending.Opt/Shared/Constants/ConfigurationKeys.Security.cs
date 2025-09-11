namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Security
  {
    public static partial class Hmac
    {
      public const string Resolver = "Security:Hmac:Resolver";
      public const string VaultUrl = "Security:Hmac:VaultUrl";
      public const string CacheSeconds = "Security:Hmac:CacheSeconds";
      public const string TenantId = "Security:Hmac:TenantId";

      public static class Credential
      {
        public const string Mode = "Security:Hmac:Credential:Mode";
        public const string TenantId = "Security:Hmac:Credential:TenantId";
        public const string ClientId = "Security:Hmac:Credential:ClientId";
      }
    }
  }
}
