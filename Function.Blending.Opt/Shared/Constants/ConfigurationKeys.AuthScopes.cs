namespace Function.Blending.Opt.Shared.Constants;

public static partial class ConfigurationKeys
{
  public static partial class Auth
  {
    public static class Scopes
    {
      public static class Quality
      {
        public const string ReadById = "Auth:Allow:Quality:ReadById";
        public const string ReadHistory = "Auth:Allow:Quality:ReadHistory";
        public const string WriteStart = "Auth:Allow:Quality:WriteStart";
        public const string ToggleState = "Auth:Allow:Quality:ToggleState";
      }
      public static class Logistics
      {
        public const string ReadById = "Auth:Allow:Logistics:ReadById";
        public const string ReadHistory = "Auth:Allow:Logistics:ReadHistory";
        public const string WriteStart = "Auth:Allow:Logistics:WriteStart";
        public const string ToggleConfirmed = "Auth:Allow:Logistics:ToggleConfirmed";
      }
    }
  }
}