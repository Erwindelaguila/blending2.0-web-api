
namespace Function.Blending.Core.Application.Interfaces.Services
{
    public interface ITokenService
    {
    
        Task<bool> ValidateTokenAsync(string jwtToken);
        string? GetClaimValue(string jwtToken, string claimType);
        List<string>? GetUserGroups(string jwtToken);
        string? GetUserName(string jwtToken);
        string? GetUserLastName(string jwtToken);
        string? GetUserObjectId(string jwtToken);
        string? GetUserEmail(string jwtToken);
        bool IsGraphToken(string jwtToken);
    }
}
