using FunctionBlending.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using Function.Blending.Upload.Models;

namespace FunctionBlending.Core.Functions
{
    public class ObtenerCadmioFunction
    {
        private readonly CadmioService _cadmioService;

        public ObtenerCadmioFunction(CadmioService cadmioService)
        {
            _cadmioService = cadmioService;
        }

        [Function("ObtenerCadmioFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload/get-cadmio-rumas")] HttpRequestData req)
        {
            var requestDto = await req.ReadFromJsonAsync<ObtenerCadmioRequestDto>();

            if (requestDto == null || requestDto.Rumas.Length == 0)
            {
                var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Debe enviar al menos una ruma.");
                return badRequest;
            }

            var result = await _cadmioService.ObtenerCadmioAsync(requestDto);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(result);
            return response;
        }
    }
}