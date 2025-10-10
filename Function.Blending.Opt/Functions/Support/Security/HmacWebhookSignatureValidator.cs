using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Function.Blending.Opt.Shared.Security;

namespace Function.Blending.Opt.Functions.Support.Security;

public sealed class HmacWebhookSignatureValidator(IKeyDecoder decoder, ILogger<HmacWebhookSignatureValidator> log) : IWebhookSignatureValidator
{
  private readonly IKeyDecoder _decoder = decoder;
  private readonly ILogger<HmacWebhookSignatureValidator> _log = log;

  // Versión rica que retorna el motivo (si la usas en el futuro)
  public HmacValidationResult Validate(string rawBody, string? secret, HttpHeadersCollection headers, string headerName)
  {
    // 1) Secret faltante -> fallo esperado (sin excepción)
    if (string.IsNullOrWhiteSpace(secret))
    {
      _log.LogWarning("HMAC: secret is null/empty.");
      return HmacValidationResult.Fail(HmacValidationError.MissingSecret);
    }

    // 2) Header de firma faltante -> fallo esperado (sin excepción)
    if (!headers.TryGetValues(headerName, out var vals))
    {
      _log.LogWarning("HMAC: header '{Header}' missing.", headerName);
      return HmacValidationResult.Fail(HmacValidationError.MissingHeader);
    }

    var provided = (vals.FirstOrDefault() ?? string.Empty).Trim();

    // 3) Decodificar clave
    ReadOnlyMemory<byte> keyBytes;
    var usedUtf8Fallback = false;
    try
    {
      // Caso feliz: Base64 válido
      keyBytes = _decoder.Decode(secret);
    }
    catch (FormatException)
    {
      // Base64 inválido -> fallo esperado, SIN excepción: usamos UTF8 como fallback
      _log.LogWarning("HMAC: secret not Base64; falling back to UTF8 bytes.");
      keyBytes = Encoding.UTF8.GetBytes(secret);
      usedUtf8Fallback = true;
    }
    catch
    {
      // Cualquier otro error de decode ES inesperado -> relanzar para que el middleware lo registre con stack
      throw;
    }

    // 4) Calcular HMAC (si algo raro falla aquí, dejamos que la excepción suba)
    var bodyBytes = Encoding.UTF8.GetBytes(rawBody ?? string.Empty);
    using var hmac = new HMACSHA256(keyBytes.ToArray());
    var expected = Convert.ToBase64String(hmac.ComputeHash(bodyBytes));

    var ok = TimingSafeEquals(expected, provided);

#if DEBUG
    if (!ok)
      _log.LogWarning("HMAC mismatch. utf8Fallback={Utf8Fallback}", usedUtf8Fallback);
#endif

    return ok
      ? HmacValidationResult.Ok
      : HmacValidationResult.Fail(usedUtf8Fallback ? HmacValidationError.InvalidSecretEncoding : HmacValidationError.Mismatch);
  }

  // Retrocompatibilidad (si tu interfaz solo expone bool)
  public bool IsValid(string rawBody, string? secret, HttpHeadersCollection headers, string headerName) => Validate(rawBody, secret, headers, headerName).IsValid;

  private static bool TimingSafeEquals(string a, string b)
  {
    var ba = Encoding.UTF8.GetBytes(a ?? "");
    var bb = Encoding.UTF8.GetBytes(b ?? "");
    if (ba.Length != bb.Length) return false;
    var diff = 0;
    for (int i = 0; i < ba.Length; i++) diff |= ba[i] ^ bb[i];
    return diff == 0;
  }
}
