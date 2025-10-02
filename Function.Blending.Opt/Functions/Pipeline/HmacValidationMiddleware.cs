using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Security;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;

namespace Function.Blending.Opt.Functions.Pipeline;

public sealed class HmacValidationMiddleware(
    ILogger<HmacValidationMiddleware> logger,
    IWebhookSignatureValidator validator,
    IHmacKeyResolver keyResolver,
    ProblemDetailsFactory pdf) : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var req = await context.GetHttpRequestDataAsync();
    if (req is null) { await next(context); return; }

    var validateAttr = GetAttr<ValidateHmacAttribute>(context);
    if (validateAttr is null) { await next(context); return; }

    if (!req.Headers.TryGetValues(HmacKeys.XKeyIdHeaderKey, out var keyVals))
    {
      await WriteProblemAsync(context, req, HttpStatusCode.BadRequest, "urn:blending:error:hmac:keyid-required", "Bad Request", $"Header '{HmacKeys.XKeyIdHeaderKey}' is required.");
      return;
    }
    var keyId = keyVals.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(keyId))
    {
      await WriteProblemAsync(context, req, HttpStatusCode.BadRequest, "urn:blending:error:hmac:keyid-required", "Bad Request", $"Header '{HmacKeys.XKeyIdHeaderKey}' is required.");
      return;
    }

    string rawBody = await ReadBodyPreserveAsync(req);

    string signatureHeaderName = string.IsNullOrWhiteSpace(validateAttr.HeaderName) ? HmacKeys.XSignatureHeaderKey : validateAttr.HeaderName;
    string secretString;
    try
    {
      var keyBytes = await keyResolver.ResolveAsync(keyId, context.CancellationToken);
      secretString = Convert.ToBase64String(keyBytes.Span);
    }
    catch (KeyNotFoundException)
    {
      await WriteProblemAsync(context, req, HttpStatusCode.Unauthorized, "urn:blending:error:hmac:unknown-keyid", "Unauthorized", $"Unknown '{HmacKeys.XKeyIdHeaderKey}'.");
      return;
    }

    var ok = validator.IsValid(rawBody, secretString, req.Headers, signatureHeaderName);
    if (!ok)
    {
      logger.LogWarning("HMAC inválido para keyId {KeyId}: firma no coincide o falta {Header}.", keyId, signatureHeaderName);
      await WriteProblemAsync(context, req, HttpStatusCode.Unauthorized, "urn:blending:error:hmac:invalid-signature", "Unauthorized", "HMAC signature header is missing or does not match the computed value.");
      return;
    }

    context.Items[HmacKeys.RawBodyItemsKey] = rawBody;
    context.Items[HmacKeys.HmacValidItemsKey] = true;
    context.Items[HmacKeys.HmacKeyIdItemsKey] = keyId;

    await next(context);
  }

  private async Task WriteProblemAsync(FunctionContext ctx, HttpRequestData req, HttpStatusCode status, string type, string title, string detail)
  {
    var res = req.CreateResponse(status);
    var corr = ctx.Items.TryGetValue(CorrelationKeys.CorrelationIdItemKey, out var v) ? v?.ToString() : null;
    var tp = ctx.Items.TryGetValue(CorrelationKeys.TraceParentIdItemKey, out var t) ? t?.ToString() : null;
    await pdf.WriteAsync(res, (int)status, title, type, detail, corr ?? tp);
    ctx.GetInvocationResult().Value = res;
  }

  private static TAttr? GetAttr<TAttr>(FunctionContext ctx) where TAttr : Attribute
  {
    try
    {
      var ep = ctx.FunctionDefinition.EntryPoint;
      if (string.IsNullOrWhiteSpace(ep)) return null;

      var lastDot = ep.LastIndexOf('.');
      if (lastDot <= 0 || lastDot >= ep.Length - 1) return null;

      var typeName = ep[..lastDot];
      var methodName = ep[(lastDot + 1)..];

      var type = AppDomain.CurrentDomain
          .GetAssemblies()
          .Select(a => a.GetType(typeName, false, false))
          .FirstOrDefault(t => t != null);

      var mi = type?.GetMethod(methodName,
          System.Reflection.BindingFlags.Public |
          System.Reflection.BindingFlags.NonPublic |
          System.Reflection.BindingFlags.Instance |
          System.Reflection.BindingFlags.Static);

      return mi?.GetCustomAttributes(typeof(TAttr), false).FirstOrDefault() as TAttr;
    }
    catch { return null; }
  }

  private static async Task<string> ReadBodyPreserveAsync(HttpRequestData req)
  {
    var body = req.Body;

    if (body.CanSeek)
    {
      body.Position = 0;
      using var reader = new StreamReader(body, Encoding.UTF8, false, 1024, true);
      var text = await reader.ReadToEndAsync();
      body.Position = 0;
      return text;
    }

    using var ms = new MemoryStream();
    await body.CopyToAsync(ms);
    var bytes = ms.ToArray();
    var textUtf8 = Encoding.UTF8.GetString(bytes);
    try { if (body.CanSeek) body.Position = 0; } catch { }
    return textUtf8;
  }
}
