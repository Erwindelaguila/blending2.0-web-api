using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Opt.Functions.Support.Http;

public static class DateConversionRequestOptions
{
  public const string ConvertKey = "convertDates";
  public const string TimeZoneKey = "tz";
  public const string HeaderTimeZone = "Accept-Timezone";

  public static (bool convert, string? tzId) From(HttpRequestData req)
  {
    var q = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

    var convertRaw = q[ConvertKey];
    bool convert = !string.IsNullOrWhiteSpace(convertRaw)
                   && bool.TryParse(convertRaw, out var b) && b;

    string? tz = q[TimeZoneKey];
    if (string.IsNullOrWhiteSpace(tz) &&
        req.Headers.TryGetValues(HeaderTimeZone, out var vals))
    {
      tz = vals.FirstOrDefault();
    }

    tz = string.IsNullOrWhiteSpace(tz) ? null : tz.Trim();
    return (convert, tz);
  }
}
