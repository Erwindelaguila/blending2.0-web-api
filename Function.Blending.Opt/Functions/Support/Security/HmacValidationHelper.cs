using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Logging;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Function.Blending.Opt.Functions.Support.Security;

public static class HmacValidationHelper
{
  /// <summary>
  /// Valida la firma HMAC de un webhook y registra en SysLog en caso de error.
  /// Devuelve null si todo es válido; si no, devuelve directamente la respuesta ProblemDetails.
  /// </summary>
  public static async Task<HttpResponseData?> ValidateAsync(
    HttpRequestData req,
    FunctionContext fctx,
    string rawBody,
    string? secret,
    string headerName,
    IWebhookSignatureValidator validator,
    ISysLogService syslog,
    IRequestContext requestContext,
    IFunctionContextAccessor fctxAccessor,
    IProblemDetailsWriter problem,
    Type sourceType,
    string methodName)
  {
    var ok = validator.IsValid(rawBody, secret, req.Headers, headerName);
    if (ok) return null;

    // 🔒 Seguridad: no logues secretos ni body completo → solo SHA256(body)
    static string BodySha256(string s)
    {
      using var h = SHA256.Create();
      return Convert.ToHexString(h.ComputeHash(Encoding.UTF8.GetBytes(s ?? ""))).ToLowerInvariant();
    }

    var composer = new SysLogComposer(requestContext, fctxAccessor, sourceType, methodName);
    var extra = $"header={headerName}; bodySha256={BodySha256(rawBody)}";
    var record = composer.FromMessage("HMAC validation failed", SysLogLevel.Warning, extra);
    await syslog.WriteAsync(record, fctx.CancellationToken);

    // ProblemDetails 401
    return await problem.CreateAsync(
      fctx, req,
      status: HttpStatusCode.Unauthorized,
      type: "urn:security:hmac:invalid",
      title: "Invalid signature",
      detail: "The webhook signature is invalid.",
      extensions: new Dictionary<string, object?> { ["reason"] = "HMAC mismatch" });
  }
}
