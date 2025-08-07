using Function.Blending.Core.Application.Menu.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Menu.Queries
{
    public class GetUserMenuQuery(string jwtToken) : IRequest<MenuResponse>
    {
        public string JwtToken { get; } = jwtToken ?? throw new ArgumentNullException(nameof(jwtToken));
    }
}
