using System.Threading.Tasks;

namespace Function.Blending.Auth.Application.Interfaces.Services
{
    public interface ITokenSignatureValidator
    {
        Task<bool> ValidateTokenSignatureAsync(string jwtToken);
    }
}
