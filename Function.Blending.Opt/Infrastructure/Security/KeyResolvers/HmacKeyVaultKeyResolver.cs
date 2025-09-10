using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Function.Blending.Opt.Infrastructure.Security.Azure;
using Function.Blending.Opt.Shared.Options.Security;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Function.Blending.Opt.Infrastructure.Security.KeyResolvers;

public sealed class HmacKeyVaultKeyResolver : IHmacKeyResolver
{
  private readonly SecretClient _client;
  private readonly IKeyDecoder _decoder;
  private readonly IMemoryCache _cache;
  private readonly HmacOptions _opt;

  public HmacKeyVaultKeyResolver(
    IOptions<HmacOptions> opt,
    IKeyDecoder decoder,
    IMemoryCache cache)
  {
    _opt = opt.Value;
    _decoder = decoder;
    _cache = cache;

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
      KeyVaultSecret secret = await _client.GetSecretAsync(keyId, cancellationToken: ct);
      var bytes = _decoder.Decode(secret.Value);
      _cache.Set(cacheKey, bytes, TimeSpan.FromSeconds(_opt.CacheSeconds));
      return bytes;
    }
    catch (RequestFailedException ex) when (ex.Status == 404)
    {
      throw new KeyNotFoundException($"Secret '{keyId}' was not found in Key Vault '{_opt.VaultUrl}'.", ex);
    }
  }
}
