
namespace Function.Blending.Core.Application.Interfaces.Services
{
  
    public interface ITokenValidator
    {
        Task<bool> ValidateTokenAsync(string jwtToken);
    }
}
