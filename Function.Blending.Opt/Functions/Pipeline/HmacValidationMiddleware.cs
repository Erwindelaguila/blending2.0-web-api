using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Security;
using Function.Blending.Opt.Shared.Options.Security;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Function.Blending.Opt.Functions.Pipeline;

public sealed class HmacValidationMiddleware : IFunctionsWorkerMiddleware
{
  private readonly ILogger<HmacValidationMiddleware> _logger;
  private readonly IWebhookSignatureValidator _validator;
  private readonly IHmacKeyResolver _keyResolver;
  private readonly ProblemDetailsFactory _pdf;
  private readonly HmacOptions _opt;

  public HmacValidationMiddleware(
      ILogger<HmacValidationMiddleware> logger,
      IWebhookSignatureValidator validator,
      IHmacKeyResolver keyResolver,
      ProblemDetailsFactory pdf,
      IOptions<HmacOptions> opt)
  {
    _logger = logger;
    _validator = validator;
    _keyResolver = keyResolver;
    _pdf = pdf;
    _opt = opt.Value;
  }

  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var req = await context.GetHttpRequestDataAsync();
    if (req is null) { await next(context); return; }

    var validateAttr = GetAttr<ValidateHmacAttribute>(context);
    if (validateAttr is null) { await next(context); return; }

    if (!req.Headers.TryGetValues("X-Key-Id", out var keyVals))
    {
      await WriteProblemAsync(context, req, HttpStatusCode.BadRequest,
        "urn:blending:error:hmac:keyid-required", "Bad Request", "Header 'X-Key-Id' is required.");
      return;
    }
    var keyId = keyVals.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(keyId))
    {
      await WriteProblemAsync(context, req, HttpStatusCode.BadRequest,
        "urn:blending:error:hmac:keyid-required", "Bad Request", "Header 'X-Key-Id' is required.");
      return;
    }

    string rawBody = await ReadBodyPreserveAsync(req);

    string signatureHeaderName = string.IsNullOrWhiteSpace(validateAttr.HeaderName) ? "X-Signature" : validateAttr.HeaderName;
    string secretString;
    try
    {
      var keyBytes = await _keyResolver.ResolveAsync(keyId, context.CancellationToken);
      secretString = Convert.ToBase64String(keyBytes.Span);
    }
    catch (KeyNotFoundException)
    {
      await WriteProblemAsync(context, req, HttpStatusCode.Unauthorized,
        "urn:blending:error:hmac:unknown-keyid", "Unauthorized", "Unknown 'X-Key-Id'.");
      return;
    }

    var ok = _validator.IsValid(rawBody, secretString, req.Headers, signatureHeaderName);
    if (!ok)
    {
      _logger.LogWarning("HMAC inválido para keyId {KeyId}: firma no coincide o falta {Header}.", keyId, signatureHeaderName);
      await WriteProblemAsync(context, req, HttpStatusCode.Unauthorized,
        "urn:blending:error:hmac:invalid-signature", "Unauthorized",
        "HMAC signature header is missing or does not match the computed value.");
      return;
    }

    context.Items["RawBody"] = rawBody;
    context.Items["HmacValid"] = true;
    context.Items["Hmac.KeyId"] = keyId;

    await next(context);
  }

  private async Task WriteProblemAsync(FunctionContext ctx, HttpRequestData req, HttpStatusCode status, string type, string title, string detail)
  {
    var res = req.CreateResponse(status);
    var traceId = ctx.Items.TryGetValue("CorrelationId", out var v) ? v?.ToString() : null;
    await _pdf.WriteAsync(res, (int)status, title, type, detail, traceId);
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
