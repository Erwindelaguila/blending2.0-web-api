using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Opt.Functions.Support.Authorization;

public interface IFunctionAttributeReader
{
  TAttr? Get<TAttr>(FunctionContext ctx) where TAttr : Attribute;
}
