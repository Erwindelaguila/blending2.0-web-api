using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Core.Application.Constants;
using Function.Blending.Core.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Function.Blending.Core.Infrastructure.Services
{
   
    public class AzureSigningKeyProvider : IAzureSigningKeyProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AzureSigningKeyProvider> _logger;
        
        private const string CACHE_KEY = "azure_signing_keys";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);
        private static readonly SemaphoreSlim Semaphore = new(1, 1);

        public AzureSigningKeyProvider(
            HttpClient httpClient,
            IMemoryCache cache,
            ILogger<AzureSigningKeyProvider> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync()
        {
           
            if (_cache.TryGetValue(CACHE_KEY, out var cachedKeys) && cachedKeys is IEnumerable<SecurityKey> keys)
            {
                _logger.LogDebug("Retrieved Azure AD signing keys from cache");
                return keys;
            }

         
            await Semaphore.WaitAsync();
            try
            {
        
                if (_cache.TryGetValue(CACHE_KEY, out cachedKeys) && cachedKeys is IEnumerable<SecurityKey> doubleCheckKeys)
                {
                    _logger.LogDebug("Retrieved Azure AD signing keys from cache (double-check)");
                    return doubleCheckKeys;
                }

                _logger.LogInformation("Fetching Azure AD signing keys from Microsoft");
                var fetchedKeys = await FetchKeysFromAzureADAsync();
                
         
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration,
                    Priority = CacheItemPriority.High,
                    Size = 1
                };

                _cache.Set(CACHE_KEY, fetchedKeys, cacheEntryOptions);
                _logger.LogInformation("Cached {KeyCount} Azure AD signing keys for {CacheHours} hours", 
                    fetchedKeys.Count(), CacheExpiration.TotalHours);

                return fetchedKeys;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        /// <summary>
        /// Fetches signing keys directly from Azure AD endpoint
        /// </summary>
        /// <returns>Collection of security keys</returns>
        private async Task<IEnumerable<SecurityKey>> FetchKeysFromAzureADAsync()
        {
            try
            {
                var keysUrl = AzureAuthConstants.AZURE_AD_KEYS_URL;
                _logger.LogDebug("Requesting Azure AD keys from: {KeysUrl}", keysUrl);
                
                var response = await _httpClient.GetStringAsync(keysUrl);
                var jwks = JsonDocument.Parse(response);
                
                var keys = new List<SecurityKey>();
                var keysArray = jwks.RootElement.GetProperty("keys");
                
                foreach (var key in keysArray.EnumerateArray())
                {
                    if (key.TryGetProperty("kty", out var kty) && kty.GetString() == "RSA")
                    {
                        try
                        {
                            var rsaKey = new RsaSecurityKey(new System.Security.Cryptography.RSAParameters
                            {
                                Modulus = Base64UrlEncoder.DecodeBytes(key.GetProperty("n").GetString()),
                                Exponent = Base64UrlEncoder.DecodeBytes(key.GetProperty("e").GetString())
                            })
                            {
                                KeyId = key.GetProperty("kid").GetString()
                            };
                            keys.Add(rsaKey);
                            
                            _logger.LogDebug("Successfully parsed RSA key with ID: {KeyId}", rsaKey.KeyId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to parse individual RSA key");
                            // Continue processing other keys
                        }
                    }
                }
                
                if (!keys.Any())
                {
                    _logger.LogWarning("No valid RSA keys found in Azure AD response");
                }
                
                return keys;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while fetching Azure AD signing keys");
                return new List<SecurityKey>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while processing Azure AD signing keys");
                return new List<SecurityKey>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching Azure AD signing keys");
                return new List<SecurityKey>();
            }
        }
    }
}
