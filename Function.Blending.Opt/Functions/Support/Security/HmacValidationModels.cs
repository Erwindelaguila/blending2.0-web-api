namespace Function.Blending.Opt.Functions.Support.Security;

public enum HmacValidationError
{
  None = 0,
  MissingSecret,
  MissingHeader,
  InvalidSecretEncoding,
  Mismatch
}

public readonly record struct HmacValidationResult(
  bool IsValid,
  HmacValidationError Error
)
{
  public static readonly HmacValidationResult Ok = new(true, HmacValidationError.None);
  public static HmacValidationResult Fail(HmacValidationError err) => new(false, err);
}
