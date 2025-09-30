using Microsoft.Azure.Functions.Worker;
using System.Reflection;

namespace Function.Blending.Core.Functions.Support.Authorization;

public sealed class FunctionAttributeReader : IFunctionAttributeReader
{
  public TAttr? Get<TAttr>(FunctionContext ctx) where TAttr : Attribute
  {
    try
    {
      var ep = ctx.FunctionDefinition.EntryPoint;
      if (string.IsNullOrWhiteSpace(ep)) return null;

      var dot = ep.LastIndexOf('.');
      if (dot <= 0 || dot >= ep.Length - 1) return null;

      var typeName = ep[..dot];
      var methodName = ep[(dot + 1)..];

      var type = AppDomain.CurrentDomain.GetAssemblies()
                     .Select(a => a.GetType(typeName, false, false))
                     .FirstOrDefault(t => t != null);

      var mi = type?.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      return mi?.GetCustomAttributes(typeof(TAttr), false).FirstOrDefault() as TAttr;
    }
    catch { return null; }
  }
}
