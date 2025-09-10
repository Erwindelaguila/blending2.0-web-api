using Function.Blending.Opt.Application.Features.CalEjecucion.Commands.Start;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Requests;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
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

namespace Function.Blending.Opt.Functions.Triggers.CalEjecucion;

public sealed class StartCalEjecucionFunction(IMediator mediator, IProblemDetailsWriter problem)
{
  [Function(nameof(StartCalEjecucionFunction))]
  [RequireScopes(ConfigurationKeys.Auth.Scopes.Quality.WriteStart)]
  public async Task<HttpResponseData> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = FunctionRoutes.Quality.Start)]
      HttpRequestData req,
      FunctionContext fctx)
  {
    // 1) Leer JSON
    var dto = await req.TryReadJsonAsync<StartCalEjecucionRequest>();
    if (dto is null)
    {
      return await problem.CreateAsync(
        fctx, req, HttpStatusCode.BadRequest,
        type: "urn:blending:error:invalid-payload",
        title: "Bad Request",
        detail: "Invalid JSON body");
    }
    // 2) Leer usuario desde Claims (oid/sub)
    var oid = fctx.GetUserObjectId();
    if (!Guid.TryParse(oid, out var creadoPorId))
    {
      return await problem.CreateAsync(
        fctx, req, HttpStatusCode.BadRequest,
        type: "urn:blending:error:invalid-user",
        title: "Invalid user id",
        detail: "The current principal does not provide a valid object id (oid/sub) to audit.");
    }

    // 3) Ejecutar comando con auditoría
    var cmd = new StartCalEjecucionCommand(dto.PlantaId, dto.Mensaje, creadoPorId)
    {
      Filtro = dto.Filtro,                // NUEVO
      Parametros = dto.Parametros         // NUEVO
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
    var location = req.BuildLocation(FunctionRoutes.Quality.GetById.Replace("{id:guid}", result.Value.Id.ToString()));
    var res = req.CreateResponse(HttpStatusCode.Created);
    res.Headers.Add("Location", location.ToString());
    await res.WriteJsonAsync(ApiResponse<StartCalEjecucionResponse>.Of(result.Value));
    return res;
  }
}
