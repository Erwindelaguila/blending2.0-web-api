using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Opt.Functions.Support.Security;

public interface IWebhookSignatureValidator
{
  bool IsValid(string rawBody, string? secret, HttpHeadersCollection headers, string headerName);
}
