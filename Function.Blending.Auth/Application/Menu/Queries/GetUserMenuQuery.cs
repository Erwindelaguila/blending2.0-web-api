using System;
using Function.Blending.Auth.Application.Menu.DTOs;
using Function.Blending.Auth.Infrastructure.Middleware;
using Function.Blending.Auth.Application.Common;
using MediatR;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Auth.Application.Menu.Queries
{
    public class GetUserMenuQuery : IRequest<HttpResponseData>
    {
        public UserClaims UserClaims { get; }
        public HttpRequestData HttpRequest { get; }

        public GetUserMenuQuery(UserClaims userClaims, HttpRequestData httpRequest)
        {
            UserClaims = userClaims ?? throw new ArgumentNullException(nameof(userClaims));
            HttpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
        }
    }
}
