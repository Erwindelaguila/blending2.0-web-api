using Function.Blending.Opt.Application.Features.LogEjecucion.Commands.Start;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using Function.Blending.Opt.Functions.Support.Authorization;
using Function.Blending.Opt.Functions.Support.Extensions;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.ProblemDetails;
using Function.Blending.Opt.Functions.Support.Routing;
using Function.Blending.Opt.Shared.Constants;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Function.Blending.Opt.Functions.Triggers.LogEjecucion
{
  public sealed class StartLogEjecucionFunction(IMediator mediator, IProblemDetailsWriter problem)
  {
    [Function(nameof(StartLogEjecucionFunction))]
    [RequireScopes(ConfigurationKeys.Auth.Scopes.Logistics.WriteStart)]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Logistics.Start)]
        HttpRequestData req,
        FunctionContext fctx)
    {
      // 1) Leer JSON
      var dto = await req.TryReadJsonAsync<StartLogEjecucionRequest>();
      if (dto is null)
      {
        return await problem.CreateAsync(
          fctx, req, HttpStatusCode.BadRequest,
          type: "urn:blending:error:invalid-payload",
          title: "Bad Request",
          detail: "Invalid JSON body");
      }

      // 2) Usuario (oid/sub)
      var oid = fctx.GetUserObjectId();
      if (!Guid.TryParse(oid, out var creadoPorId))
      {
        return await problem.CreateAsync(
          fctx, req, HttpStatusCode.BadRequest,
          type: "urn:blending:error:invalid-user",
          title: "Invalid user id",
          detail: "The current principal does not provide a valid object id (oid/sub) to audit.");
      }

      // 3) Command con inputs (profundo)
      var cmd = new StartLogEjecucionCommand(dto.Mensaje, creadoPorId)
      {
        Info = dto.Info,
        Filtro = dto.Filtro,
        Oferta = dto.Oferta
      };

      var result = await mediator.Send(cmd);

      if (!result.IsSuccess || result.Value is null)
      {
        var detail = result.Error ?? "Unknown error";
        return await problem.CreateAsync(
          fctx, req, HttpStatusCode.BadRequest,
          type: "urn:blending:error:start-failed",
          title: "Start Failed",
          detail: detail);
      }

      // 4) 201 + Location → GetById
      var location = req.BuildLocation(
        FunctionRoutes.Logistics.GetById.Replace("{id:guid}", result.Value.Id.ToString())
      );

      var res = req.CreateResponse(HttpStatusCode.Created);
      res.Headers.Add("Location", location.ToString());
      await res.WriteJsonAsync(ApiResponse<StartLogEjecucionResponse>.Of(result.Value));
      return res;
    }
  }
}
