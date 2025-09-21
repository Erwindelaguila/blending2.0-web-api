using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Upload.Functions.Support.Authorization;

public interface IFunctionAttributeReader
{
  TAttr? Get<TAttr>(FunctionContext ctx) where TAttr : Attribute;
}
