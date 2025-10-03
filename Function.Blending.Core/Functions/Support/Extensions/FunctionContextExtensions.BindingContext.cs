using System.Security.Claims;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Core.Functions.Support.Extensions.BindingContext.BindingData;

public static partial class FunctionContextExtensions
{

  public static string? TryStringBindingData(this FunctionContext ctx, string key) => ctx.BindingContext.BindingData.TryGetValue(key, out var v) ? v?.ToString() : null;

  public static Guid? TryGuidBindingData(this FunctionContext ctx, string key)
  {
    var valueRaw = ctx.TryStringBindingData(key);
    if (valueRaw == null) return null;

    if (!Guid.TryParse(valueRaw, out Guid value)) return null;

    return value;
  }
  public static int? TryIntBindingData(this FunctionContext ctx, string key)
  {
    var valueRaw = ctx.TryStringBindingData(key);
    if (valueRaw == null) return null;

    if (!int.TryParse(valueRaw, out int value)) return null;

    return value;
  }
  public static bool? TryBoolBindingData(this FunctionContext ctx, string key)
  {
    var valueRaw = ctx.TryStringBindingData(key);
    if (valueRaw == null) return null;

    if (!bool.TryParse(valueRaw, out bool value)) return null;

    return value;
  }
}
