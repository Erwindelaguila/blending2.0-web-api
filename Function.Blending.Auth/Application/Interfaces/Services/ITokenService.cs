namespace Function.Blending.Auth.Application.Interfaces.Services
{
   
    public interface ITokenService
    {
        
        Task<bool> ValidateTokenAsync(string jwtToken);
        
        
        string? GetUserObjectId(string jwtToken);
        

        string? GetUserName(string jwtToken);
      
        List<string>? GetUserGroups(string jwtToken);
    }
}