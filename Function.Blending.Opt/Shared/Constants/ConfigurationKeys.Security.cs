namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Security
  {
    public static partial class Hmac
    {
      public const string Resolver = "Security_Hmac_Resolver";
      public const string VaultUrl = "Security_Hmac_VaultUrl";
      public const string CacheSeconds = "Security_Hmac_CacheSeconds";
      public const string TenantId = "Security_Hmac_TenantId";

      public static class Credential
      {
        public const string Mode = "Security_Hmac_Credential_Mode";
        public const string TenantId = "Security_Hmac_Credential_TenantId";
        public const string ClientId = "Security_Hmac_Credential_ClientId";
      }
      public static class Fallback
      {
        public const string Enable = "Security_Hmac_Fallback_Enable";
        public const string AllowList = "Security_Hmac_Fallback_AllowList";
        public const string SecretsPrefix = "Security_Hmac_Fallback_SecretsPrefix";
      }
    }
  }
}
