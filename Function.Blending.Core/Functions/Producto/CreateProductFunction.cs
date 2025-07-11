using Function.Blending.Core.Application.Products.Commands;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;
using FluentValidation;

public class CreateProductFunction
{
    private readonly IMediator _mediator;

    public CreateProductFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Function("CreateProduct")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "productos")] HttpRequestData req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var command = JsonSerializer.Deserialize<CreateProductCommand>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var result = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(result);
            return response;
        }
        catch (ValidationException ex)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToList();
            await response.WriteAsJsonAsync(new
            {
                Message = "Validation Failed",
                Errors = errors
            });
            return response;
        }
        catch (Exception ex)
        {
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);

            var errorMessage = new
            {
                Message = "Ocurrió un error inesperado.",
                Exception = ex.Message,
                InnerException = ex.InnerException?.Message,
                StackTrace = ex.StackTrace
            };

            await response.WriteAsJsonAsync(errorMessage);
            return response;
        }
    }
}