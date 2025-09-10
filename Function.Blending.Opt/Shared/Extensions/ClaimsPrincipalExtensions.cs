using System.Security.Claims;
using Function.Blending.Opt.Shared.Constants;

namespace Function.Blending.Opt.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
  public static IEnumerable<string> GetValues(this ClaimsPrincipal? principal, params string[] types)
  {
    if (principal is null) yield break;

    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var c in principal.Claims)
    {
      if (types.Contains(c.Type, StringComparer.OrdinalIgnoreCase) && set.Add(c.Value))
        yield return c.Value;
    }
  }

  public static IEnumerable<string> GetGroupValues(this ClaimsPrincipal? principal)
    => principal.GetValues(ClaimTypesEx.GroupClaimTypes);

  public static string? GetFirstValue(this ClaimsPrincipal? principal, params string[] types)
    => principal?.Claims.FirstOrDefault(c => types.Contains(c.Type, StringComparer.OrdinalIgnoreCase))?.Value;

  public static string? GetUserId(this ClaimsPrincipal? principal)
    => principal.GetFirstValue(ClaimTypesEx.UserIdClaimTypes);

  public static string? GetUsername(this ClaimsPrincipal? principal)
    => principal.GetFirstValue(ClaimTypesEx.UsernameClaimTypes);

  public static bool HasAnyGroup(this ClaimsPrincipal? principal, IEnumerable<string>? groupsCsvOrList)
  {
    var groups = groupsCsvOrList?.ToArray() ?? Array.Empty<string>();
    if (groups.Length == 0) return false;
    var mine = new HashSet<string>(principal.GetGroupValues(), StringComparer.OrdinalIgnoreCase);
    return groups.Any(mine.Contains);
  }

  public static string? GetCsvGroupValues(this ClaimsPrincipal? principal) => principal.GetGroupValues().Any() ? string.Join(",", principal.GetGroupValues()) : null;
}
