using Microsoft.IdentityModel.Tokens;

namespace Function.Blending.Core.Application.Interfaces.Services
{

    public interface IAzureSigningKeyProvider
    {

        Task<IEnumerable<SecurityKey>> GetSigningKeysAsync();
    }
}
