using Function.Blending.Auth.Application.Menu.DTOs;

namespace Function.Blending.Auth.Application.Interfaces.Services;

public interface IMenuService
{
    Task<MenuData> BuildUserMenuAsync(string userId, string userName, List<string> userRoles);
}
