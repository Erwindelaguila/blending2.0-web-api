using System;
using System.Net;
using System.Threading.Tasks;
using Function.Blending.Auth.Application.Constants;
using Function.Blending.Auth.Application.Menu.Queries;
using Function.Blending.Auth.Infrastructure.Extensions;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Functions.Menu
{
    public class GetUserMenuFunction
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GetUserMenuFunction> _logger;

        public GetUserMenuFunction(
            IMediator mediator,
            ILogger<GetUserMenuFunction> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Function(FunctionNames.User.GetMenu)]
        public async Task<HttpResponseData> GetUserMenu(
            [HttpTrigger(AuthorizationLevel.Anonymous, HttpMethods.Get, Route = ApiRoutes.Auth.User.Menu)] HttpRequestData req,
            FunctionContext context)
        {
            _logger.LogInformation("Procesando solicitud de menú");
            var userClaims = context.GetUserClaims()!;
            var query = new GetUserMenuQuery(userClaims, req);
            
            var httpResponse = await _mediator.Send(query);
            return httpResponse;
        }
    }
}
