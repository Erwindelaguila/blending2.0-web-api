using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Core.Infrastructure.Security.Support.Authorization;

public interface IFunctionAttributeReader
{
  TAttr? Get<TAttr>(FunctionContext ctx) where TAttr : Attribute;
}
