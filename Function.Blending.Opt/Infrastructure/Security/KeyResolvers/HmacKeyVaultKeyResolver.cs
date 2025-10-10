using Azure;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Function.Blending.Opt.Infrastructure.Security.Azure;
using Function.Blending.Opt.Shared.Options.Security;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

// SysLog (opcional)
using Function.Blending.Opt.Domain.Abstractions.Services;   // ISysLogService
using Function.Blending.Opt.Domain.Logging;                 // SysLogRecord, SysLogLevel

namespace Function.Blending.Opt.Infrastructure.Security.KeyResolvers;

public sealed class HmacKeyVaultKeyResolver : IHmacKeyResolver
{
  private readonly SecretClient _client;
  private readonly IKeyDecoder _decoder;
  private readonly IMemoryCache _cache;
  private readonly HmacOptions _opt;
  private readonly ILogger<HmacKeyVaultKeyResolver> _logger;
  private readonly ISysLogService? _syslog;

  private static readonly ConcurrentDictionary<string, bool> _allow = new();

  public HmacKeyVaultKeyResolver(
    IOptions<HmacOptions> opt,
    IKeyDecoder decoder,
    IMemoryCache cache,
    ILogger<HmacKeyVaultKeyResolver> logger,
    ISysLogService? syslog = null // <- opcional
  )
  {
    _opt = opt.Value;
    _decoder = decoder;
    _cache = cache;
    _logger = logger;
    _syslog = syslog;

    TokenCredential cred = TokenCredentialFactory.Create(_opt);
    _client = new SecretClient(new Uri(_opt.VaultUrl), cred);
  }

  public async Task<ReadOnlyMemory<byte>> ResolveAsync(string keyId, CancellationToken ct = default)
  {
    var cacheKey = $"hmac:kv:{keyId}";
    if (_cache.TryGetValue(cacheKey, out ReadOnlyMemory<byte> hit) && !hit.IsEmpty)
      return hit;

    try
    {
      // 1) Key Vault (camino principal)
      KeyVaultSecret secret = await _client.GetSecretAsync(keyId, cancellationToken: ct);
      var bytes = _decoder.Decode(secret.Value);
      _cache.Set(cacheKey, bytes, TimeSpan.FromSeconds(_opt.CacheSeconds));
      return bytes;
    }
    catch (RequestFailedException ex) when (ex.Status == 404)
    {
      _logger.LogWarning(ex, "Secret '{KeyId}' not found in KeyVault '{VaultUrl}'.", keyId, _opt.VaultUrl);

      if (TryResolveFromFallback(keyId, out var fbBytes))
      {
        _logger.LogWarning("Using FALLBACK HMAC for '{KeyId}' (KV 404).", keyId);
        SafeSysLog(
          level: SysLogLevel.Warning,
          method: nameof(ResolveAsync),
          message: $"Fallback HMAC used (404) for '{keyId}'.",
          extra: $"vault={_opt.VaultUrl}"
        );
        _cache.Set(cacheKey, fbBytes, TimeSpan.FromSeconds(_opt.CacheSeconds));
        return fbBytes;
      }

      SafeSysLog(
        level: SysLogLevel.Error,
        method: nameof(ResolveAsync),
        message: $"Secret '{keyId}' not found in KeyVault.",
        extra: $"vault={_opt.VaultUrl}; status=404"
      );
      throw new KeyNotFoundException($"Secret '{keyId}' was not found in Key Vault '{_opt.VaultUrl}'.", ex);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error resolving HMAC secret '{KeyId}' from KeyVault '{VaultUrl}'.", keyId, _opt.VaultUrl);

      if (TryResolveFromFallback(keyId, out var fbBytes))
      {
        _logger.LogWarning("Using FALLBACK HMAC for '{KeyId}' due to KV error.", keyId);
        SafeSysLog(
          level: SysLogLevel.Warning,
          method: nameof(ResolveAsync),
          message: $"Fallback HMAC used (KV error) for '{keyId}'.",
          extra: $"vault={_opt.VaultUrl}; ex={ex.GetType().Name}"
        );
        _cache.Set(cacheKey, fbBytes, TimeSpan.FromSeconds(_opt.CacheSeconds));
        return fbBytes;
      }

      SafeSysLog(
        level: SysLogLevel.Error,
        method: nameof(ResolveAsync),
        message: $"KV error resolving '{keyId}'.",
        extra: $"vault={_opt.VaultUrl}; ex={ex}"
      );
      throw;
    }
  }

  private bool TryResolveFromFallback(string keyId, out ReadOnlyMemory<byte> bytes)
  {
    bytes = ReadOnlyMemory<byte>.Empty;

    // master switch
    var enabled = _opt.Fallback.Enable;
    if (!enabled) return false;

    // allow list opcional (coma-separado)
    var allowList = _opt.Fallback.AllowList;
    if (!string.IsNullOrWhiteSpace(allowList))
    {
      var allowed = _allow.GetOrAdd(keyId, k =>
        allowList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                 .Any(x => string.Equals(x, k, StringComparison.OrdinalIgnoreCase)));

      if (!allowed) return false;
    }

    // nombre de la setting: Security_Hmac_Fallback_Secrets__{keyId}
    var prefix = _opt.Fallback.SecretsPrefix;
    var varName = $"{prefix}{keyId}";
    var raw = Environment.GetEnvironmentVariable(varName);
    if (string.IsNullOrWhiteSpace(raw)) return false;

    try
    {
      bytes = _decoder.Decode(raw);
      return !bytes.IsEmpty;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Fallback value for '{KeyId}' is invalid (cannot decode).", keyId);
      SafeSysLog(
        level: SysLogLevel.Error,
        method: nameof(TryResolveFromFallback),
        message: $"Invalid fallback secret for '{keyId}'.",
        extra: ex.ToString()
      );
      return false;
    }
  }

  private void SafeSysLog(SysLogLevel level, string method, string message, string? extra = null)
  {
    if (_syslog is null) return;

    try
    {
      var entry = new SysLogRecord(
        Id: Guid.Empty,
        NameSpace: typeof(HmacKeyVaultKeyResolver).Namespace ?? "Function.Blending.Opt",
        ClassName: nameof(HmacKeyVaultKeyResolver),
        MethodName: method,
        Username: null,
        UserId: null,
        Message: message,
        StackTrace: null,
        ExtraInfo: extra,
        TraceParentId: null,
        RequestInvocationId: null,
        FunctionInvocationId: null,
        ExceptionGroupId: null,
        Level: level
      );

      // fire-and-forget (no bloquear la resolución de clave por logging)
      _ = _syslog.WriteAsync(entry);
    }
    catch
    {
      // jamás romper por logging
    }
  }
}
