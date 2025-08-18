using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Blending.Core.Application.Interfaces.Services
{
    public interface IAzureAppConfigService
    {
        Task<string> GetValueAsync(string key, string? label = null);
        Task<Dictionary<string, string>> GetValuesAsync(string keyFilter, string? label = null);
        Task<bool> SetValueAsync(string key, string value, string? label = null, string? contentType = null);
        Task<bool> DeleteValueAsync(string key, string? label = null);
        bool IsConfigured { get; }
    }
}
