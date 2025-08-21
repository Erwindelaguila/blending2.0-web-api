namespace Function.Blending.Auth.Application.Interfaces.Services;

public interface IRoleService
{
    Task<List<string>> GetUserRolesAsync(List<string> userGroups);
}
