using System.Collections;
using System.Reflection;
using Function.Blending.Opt.Domain.Abstractions.Services;

namespace Function.Blending.Opt.Application.Support.Time;

public static class DateTimeGraphLocalizer
{
  private sealed class RefEq : IEqualityComparer<object>
  {
    public static readonly RefEq Instance = new();
    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
    public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
  }

  private static readonly Dictionary<Type, PropertyInfo[]> _propsCache = new();

  public static void ConvertUtcToLocal(object? root, ITimeZoneService tz) =>
    ConvertUtcToLocal(root, tz, tzOverride: null);

  public static void ConvertUtcToLocal(object? root, ITimeZoneService tz, TimeZoneInfo? tzOverride)
  {
    if (root is null) return;
    var visited = new HashSet<object>(RefEq.Instance);
    Visit(root, tz, tzOverride, visited);
  }

  private static void Visit(object obj, ITimeZoneService tz, TimeZoneInfo? tzOverride, HashSet<object> visited)
  {
    var type = obj.GetType();
    if (type.IsPrimitive || type == typeof(string)) return;

    if (!type.IsValueType)
    {
      if (visited.Contains(obj)) return;
      visited.Add(obj);
    }

    if (obj is IEnumerable seq && type != typeof(byte[]))
    {
      foreach (var item in seq)
        if (item is not null) Visit(item, tz, tzOverride, visited);
      return;
    }

    foreach (var p in GetConvertibleProps(type))
    {
      if (!p.CanRead || !p.CanWrite) continue;
      var current = p.GetValue(obj);
      if (current is null) continue;

      var pt = p.PropertyType;

      if (pt == typeof(DateTimeOffset))
        p.SetValue(obj, ToLocal(tz, tzOverride, (DateTimeOffset)current));
      else if (pt == typeof(DateTimeOffset?))
      {
        var ndto = (DateTimeOffset?)current;
        if (ndto.HasValue) p.SetValue(obj, (DateTimeOffset?)ToLocal(tz, tzOverride, ndto.Value));
      }
      else if (pt == typeof(DateTime))
        p.SetValue(obj, ToLocalDateTime(tz, tzOverride, (DateTime)current));
      else if (pt == typeof(DateTime?))
      {
        var ndt = (DateTime?)current;
        if (ndt.HasValue) p.SetValue(obj, (DateTime?)ToLocalDateTime(tz, tzOverride, ndt.Value));
      }
      else
        Visit(current, tz, tzOverride, visited);
    }
  }

  private static PropertyInfo[] GetConvertibleProps(Type t)
  {
    if (_propsCache.TryGetValue(t, out var cached)) return cached;

    var props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
      .Where(p => p.GetIndexParameters().Length == 0)
      .Where(p =>
           p.PropertyType == typeof(DateTime) ||
           p.PropertyType == typeof(DateTime?) ||
           p.PropertyType == typeof(DateTimeOffset) ||
           p.PropertyType == typeof(DateTimeOffset?) ||
          (!p.PropertyType.IsPrimitive &&
            p.PropertyType != typeof(string) &&
            p.PropertyType != typeof(byte[])))
      .ToArray();

    _propsCache[t] = props;
    return props;
  }

  private static DateTimeOffset ToLocal(ITimeZoneService tz, TimeZoneInfo? tzOverride, DateTimeOffset source)
  {
    var utc = source.Offset == TimeSpan.Zero ? source : source.ToUniversalTime();

    if (tzOverride is null)
      return tz.ToLocal(utc);

    var local = TimeZoneInfo.ConvertTime(utc.UtcDateTime, tzOverride);
    return new DateTimeOffset(local, tzOverride.GetUtcOffset(local));
  }

  private static DateTime ToLocalDateTime(ITimeZoneService tz, TimeZoneInfo? tzOverride, DateTime source)
  {
    var utc = source.Kind == DateTimeKind.Utc ? source : DateTime.SpecifyKind(source, DateTimeKind.Utc);
    var dtoLocal = ToLocal(tz, tzOverride, new DateTimeOffset(utc, TimeSpan.Zero));
    // Devolvemos DateTime “desvinculado” (sin Kind/offset) para UI/Excel
    return dtoLocal.DateTime;
  }
}
