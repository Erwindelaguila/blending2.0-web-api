using Function.Blending.Auth.Application.Menu.DTOs;
using MediatR;

namespace Function.Blending.Auth.Application.Menu.Queries
{
    /// <summary>
    /// Query para obtener el menú del usuario basado en información de headers APIM
    /// Ya no necesita JWT porque APIM ya validó la autenticación
    /// </summary>
    public class GetUserMenuQuery : IRequest<MenuResponse>
    {
        public string UserId { get; }
        public string UserName { get; }
        public List<string> UserGroups { get; }

        public GetUserMenuQuery(string userId, string userName, List<string> userGroups)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            UserGroups = userGroups ?? throw new ArgumentNullException(nameof(userGroups));
        }
    }
}
