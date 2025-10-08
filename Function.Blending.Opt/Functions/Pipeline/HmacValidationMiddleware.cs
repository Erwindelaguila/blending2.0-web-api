using System.Net;
using System.Security.Cryptography;
using System.Text;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.Logging; // SysLogComposer
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Security;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Opt.Functions.Pipeline;

public sealed class HmacValidationMiddleware(
    ILogger<HmacValidationMiddleware> logger,
    IWebhookSignatureValidator validator,
    IHmacKeyResolver keyResolver,
    ProblemDetailsFactory pdf,
    ISysLogService syslog,
    IRequestContext requestContext,
    IFunctionContextAccessor fctxAccessor
) : IFunctionsWorkerMiddleware
{
  public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
  {
    var req = await context.GetHttpRequestDataAsync();
    if (req is null) { await next(context); return; }

    var validateAttr = GetAttr<ValidateHmacAttribute>(context);
    if (validateAttr is null) { await next(context); return; }

    // Lee y preserva el body (lo necesitamos para hash en log y para la Function)
    string rawBody = await ReadBodyPreserveAsync(req);

    // === 1) X-Key-Id obligatorio (fallo esperado → Warning, sin excepción) ===
    if (!req.Headers.TryGetValues(HmacKeys.XKeyIdHeaderKey, out var keyVals))
    {
      await FailExpectedAsync(context, req,
        status: HttpStatusCode.BadRequest,
        type: "urn:blending:error:hmac:keyid-required",
        title: "Bad Request",
        detail: $"Header '{HmacKeys.XKeyIdHeaderKey}' is required.",
        keyId: null,
        rawBody: rawBody,
        reason: "MissingKeyId",
        signatureHeaderName: validateAttr.HeaderName);
      return;
    }

    var keyId = keyVals.FirstOrDefault();
    if (string.IsNullOrWhiteSpace(keyId))
    {
      await FailExpectedAsync(context, req,
        status: HttpStatusCode.BadRequest,
        type: "urn:blending:error:hmac:keyid-required",
        title: "Bad Request",
        detail: $"Header '{HmacKeys.XKeyIdHeaderKey}' is required.",
        keyId: null,
        rawBody: rawBody,
        reason: "EmptyKeyId",
        signatureHeaderName: validateAttr.HeaderName);
      return;
    }

    // === 2) Resolver secreto (fallos con excepción → Error con StackTrace) ===
    string secretString;
    try
    {
      var keyBytes = await keyResolver.ResolveAsync(keyId, context.CancellationToken);
      // el validador espera string; la clave real NO se loguea nunca
      secretString = Convert.ToBase64String(keyBytes.Span);
    }
    catch (KeyNotFoundException ex)
    {
      await FailWithExceptionAsync(context, req, ex,
        status: HttpStatusCode.Unauthorized,
        type: "urn:blending:error:hmac:unknown-keyid",
        title: "Unauthorized",
        detail: $"Unknown '{HmacKeys.XKeyIdHeaderKey}'.",
        keyId: keyId,
        rawBody: rawBody,
        reason: "UnknownKeyId",
        signatureHeaderName: validateAttr.HeaderName);
      return;
    }
    catch (Exception ex) // timeouts, auth MSI, red, etc.
    {
      await FailWithExceptionAsync(context, req, ex,
        status: HttpStatusCode.Unauthorized,
        type: "urn:blending:error:hmac:key-resolver-failure",
        title: "Unauthorized",
        detail: "Unable to resolve HMAC key.",
        keyId: keyId,
        rawBody: rawBody,
        reason: "KeyResolverException",
        signatureHeaderName: validateAttr.HeaderName);
      return;
    }

    // === 3) Validar firma (fallo esperado → Warning; si el validador lanza → Error) ===
    string signatureHeaderName = string.IsNullOrWhiteSpace(validateAttr.HeaderName)
      ? HmacKeys.XSignatureHeaderKey
      : validateAttr.HeaderName;

    bool ok;
    try
    {
      ok = validator.IsValid(rawBody, secretString, req.Headers, signatureHeaderName);
    }
    catch (Exception ex)
    {
      // Si tu validador (o decoder interno) lanza, aquí registramos la excepción real con stack.
      await FailWithExceptionAsync(context, req, ex,
        status: HttpStatusCode.Unauthorized,
        type: "urn:blending:error:hmac:validator-exception",
        title: "Unauthorized",
        detail: "HMAC validator failed unexpectedly.",
        keyId: keyId,
        rawBody: rawBody,
        reason: "ValidatorException",
        signatureHeaderName: signatureHeaderName);
      return;
    }

    if (!ok)
    {
      logger.LogWarning("HMAC inválido para keyId {KeyId}: firma no coincide o falta {Header}.", keyId, signatureHeaderName);

      await FailExpectedAsync(context, req,
        status: HttpStatusCode.Unauthorized,
        type: "urn:blending:error:hmac:invalid-signature",
        title: "Unauthorized",
        detail: "HMAC signature header is missing or does not match the computed value.",
        keyId: keyId,
        rawBody: rawBody,
        reason: "InvalidSignature",
        signatureHeaderName: signatureHeaderName);
      return;
    }

    // === 4) OK → propagar datos útiles a la Function ===
    context.Items[HttpRequestDataKeys.RawBodyItemsKey] = rawBody;
    context.Items[HmacKeys.HmacValidItemsKey] = true;
    context.Items[HmacKeys.HmacKeyIdItemsKey] = keyId;

    await next(context);
  }

  // ==================== Helpers ====================

  // Fallos "esperados" (sin excepción): faltan headers, firma no coincide
  private async Task FailExpectedAsync(
    FunctionContext ctx,
    HttpRequestData req,
    HttpStatusCode status,
    string type,
    string title,
    string detail,
    string? keyId,
    string rawBody,
    string reason,
    string? signatureHeaderName)
  {
    try
    {
      var composer = new SysLogComposer(
        requestContext, fctxAccessor,
        sourceType: ResolveSourceType(ctx) ?? typeof(HmacValidationMiddleware),
        methodName: ResolveMethodName(ctx));

      var extra = $"reason={reason}; keyId={(keyId ?? "<null>")}; header={(signatureHeaderName ?? HmacKeys.XSignatureHeaderKey)}; bodySha256={Sha256Hex(rawBody)}";
      var rec = composer.FromMessage("HMAC validation failed", SysLogLevel.Warning, extra);
      await syslog.WriteAsync(rec, ctx.CancellationToken);
    }
    catch { /* nunca bloquear por logging */ }

    await WriteProblemAsync(ctx, req, status, type, title, detail);
  }

  // Fallos con excepción: registran StackTrace/Message en DB
  private async Task FailWithExceptionAsync(
    FunctionContext ctx,
    HttpRequestData req,
    Exception ex,
    HttpStatusCode status,
    string type,
    string title,
    string detail,
    string? keyId,
    string rawBody,
    string reason,
    string? signatureHeaderName)
  {
    try
    {
      var composer = new SysLogComposer(
        requestContext, fctxAccessor,
        sourceType: ResolveSourceType(ctx) ?? typeof(HmacValidationMiddleware),
        methodName: ResolveMethodName(ctx));

      var extra = $"reason={reason}; keyId={(keyId ?? "<null>")}; header={(signatureHeaderName ?? HmacKeys.XSignatureHeaderKey)}; bodySha256={Sha256Hex(rawBody)}";
      // 👇 AQUI va la excepción (StackTrace + Message) a DDBB:
      var rec = composer.FromException(ex, SysLogLevel.Error, extra);
      await syslog.WriteAsync(rec, ctx.CancellationToken);
    }
    catch { /* nunca bloquear por logging */ }

    await WriteProblemAsync(ctx, req, status, type, title, detail);
  }

  private async Task WriteProblemAsync(FunctionContext ctx, HttpRequestData req, HttpStatusCode status, string type, string title, string detail)
  {
    var res = req.CreateResponse(status);
    var corr = ctx.Items.TryGetValue(CorrelationKeys.CorrelationIdItemKey, out var v) ? v?.ToString() : null;
    var tp = ctx.Items.TryGetValue(CorrelationKeys.TraceParentIdItemKey, out var t) ? t?.ToString() : null;
    await pdf.WriteAsync(res, (int)status, title, type, detail, corr ?? tp);
    ctx.GetInvocationResult().Value = res; // corta ejecución
  }

  private static string Sha256Hex(string s)
  {
    using var h = SHA256.Create();
    return Convert.ToHexString(h.ComputeHash(Encoding.UTF8.GetBytes(s ?? ""))).ToLowerInvariant();
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

  private static Type? ResolveSourceType(FunctionContext ctx)
  {
    try
    {
      var ep = ctx.FunctionDefinition.EntryPoint;
      var lastDot = ep.LastIndexOf('.');
      if (lastDot < 0) return null;
      var typeName = ep[..lastDot];

      var asmPath = ctx.FunctionDefinition.PathToAssembly;
      var asm = System.Reflection.Assembly.LoadFrom(asmPath);
      return asm.GetType(typeName, throwOnError: false);
    }
    catch { return null; }
  }

  private static string? ResolveMethodName(FunctionContext ctx)
  {
    try
    {
      var ep = ctx.FunctionDefinition.EntryPoint;
      var lastDot = ep.LastIndexOf('.');
      return lastDot < 0 ? null : ep[(lastDot + 1)..];
    }
    catch { return null; }
  }
}
