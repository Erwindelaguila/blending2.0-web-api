namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Auth
  {
    public static class Scopes
    {
      public static class Quality
      {
        public const string ReadById = "Auth_Allow_Quality_ReadById";
        public const string ReadHistory = "Auth_Allow_Quality_ReadHistory";
        public const string WriteStart = "Auth_Allow_Quality_WriteStart";
        public const string ChangeAccepted = "Auth_Allow_Quality_ChangeAccepted";
        public const string ReadParameters = "Auth_Allow_Quality_ReadParameters";
        public const string ReadInputById = "Auth_Allow_Quality_ReadInputById";
        public const string ReadOutputById = "Auth_Allow_Quality_ReadOutputById";
      }
      public static class Logistics
      {
        public const string ReadById = "Auth_Allow_Logistics_ReadById";
        public const string ReadHistory = "Auth_Allow_Logistics_ReadHistory";
        public const string WriteStart = "Auth_Allow_Logistics_WriteStart";
        public const string ToggleConfirmed = "Auth_Allow_Logistics_ToggleConfirmed";
        public const string ReadInputById = "Auth_Allow_Logistics_ReadInputById";
        public const string ReadOutputById = "Auth_Allow_Logistics_ReadOutputById";
      }
    }
  }
}