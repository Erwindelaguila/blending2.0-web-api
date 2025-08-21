using Function.Blending.Auth.Application.Menu.DTOs;
using MediatR;

namespace Function.Blending.Auth.Application.Menu.Queries
{
    public class GetUserMenuQuery(string jwtToken) : IRequest<MenuResponse>
    {
        public string JwtToken { get; } = jwtToken ?? throw new ArgumentNullException(nameof(jwtToken));
    }
}
