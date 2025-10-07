namespace Function.Blending.Opt.Application.Support.Meta;

public sealed record DateConversionMeta(
  bool Converted,
  string EffectiveTimeZoneId,
  bool OverrideApplied
);
