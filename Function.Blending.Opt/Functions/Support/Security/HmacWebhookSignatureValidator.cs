using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using Function.Blending.Opt.Shared.Security; // IKeyDecoder

namespace Function.Blending.Opt.Functions.Support.Security;

/// <summary>
/// Valida la firma HMAC-SHA256 del cuerpo (body-only).
/// La clave viene como texto (normalmente Base64) y aquí se DECODIFICA a bytes.
/// </summary>
public sealed class HmacWebhookSignatureValidator : IWebhookSignatureValidator
{
  private readonly IKeyDecoder _decoder;
  private readonly ILogger<HmacWebhookSignatureValidator> _log;

  // Asegúrate que en Program.cs tengas: services.AddSingleton<IKeyDecoder, KeyDecoder>();
  public HmacWebhookSignatureValidator(IKeyDecoder decoder, ILogger<HmacWebhookSignatureValidator> log)
  {
    _decoder = decoder;
    _log = log;
  }

  /// <param name="rawBody">Body EXACTO (texto) recibido por la request.</param>
  /// <param name="secret">Clave simétrica tal como viene del resolver (normalmente Base64).</param>
  /// <param name="headers">Headers de la request.</param>
  /// <param name="headerName">Nombre del header de firma (p.ej. "X-Signature").</param>
  public bool IsValid(string rawBody, string? secret, HttpHeadersCollection headers, string headerName)
  {
    if (string.IsNullOrWhiteSpace(secret))
    {
      _log.LogWarning("HMAC: secret is null/empty.");
      return false;
    }

    if (!headers.TryGetValues(headerName, out var vals))
    {
      _log.LogWarning("HMAC: header '{Header}' missing.", headerName);
      return false;
    }

    var provided = vals.FirstOrDefault() ?? string.Empty;

    // --- DECODIFICAR la clave (Base64 -> bytes). Si no es Base64 válida, usamos UTF8 como fallback. ---
    ReadOnlyMemory<byte> keyBytes;
    try
    {
      keyBytes = _decoder.Decode(secret); // <- usa Convert.FromBase64String por dentro
    }
    catch
    {
      // Si por alguna razón no es Base64 válida, caemos a bytes de la cadena (evita hard-fail).
      _log.LogWarning("HMAC: secret not Base64; falling back to UTF8 bytes.");
      keyBytes = Encoding.UTF8.GetBytes(secret);
    }

    // --- HMAC-SHA256 sobre el body tal cual llegó (UTF8) ---
    var bodyBytes = Encoding.UTF8.GetBytes(rawBody ?? string.Empty);
    using var hmac = new HMACSHA256(keyBytes.ToArray());
    var expectedSig = Convert.ToBase64String(hmac.ComputeHash(bodyBytes));

    var ok = TimingSafeEquals(expectedSig, provided);

#if DEBUG
    if (!ok)
      _log.LogWarning("HMAC mismatch. expected={Expected} provided={Provided}", expectedSig, provided);
#endif

    return ok;
  }

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
