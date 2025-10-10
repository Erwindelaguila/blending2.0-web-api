using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Function.Blending.Auth.Application.Common.Wrappers;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class HttpResponseService : IHttpResponseService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public async Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData request, string message, HttpStatusCode statusCode)
        {
            var errorResponse = BaseResponse<object>.Fail(message, (int)statusCode);
            var response = request.CreateResponse(statusCode);
            
            SetJsonContentType(response);
            
            var json = JsonSerializer.Serialize(errorResponse, JsonOptions);
            await response.WriteStringAsync(json);
            
            return response;
        }

        public async Task<HttpResponseData> CreateSuccessResponseAsync<T>(HttpRequestData request, T data, string? message = null)
        {
            var successResponse = BaseResponse<T>.Success(data, message);
            var response = request.CreateResponse(HttpStatusCode.OK);
            
            SetJsonContentType(response);
            
            var json = JsonSerializer.Serialize(successResponse, JsonOptions);
            await response.WriteStringAsync(json);
            
            return response;
        }

        public async Task<HttpResponseData> ToHttpResponseAsync<T>(HttpRequestData request, Application.Common.Results.Result<T> result)
        {
            if (result.IsSuccess)
            {
                return await CreateSuccessResponseAsync(request, result.Data);
            }
            else
            {
                return await CreateErrorResponseAsync(request, result.ErrorMessage ?? "Error desconocido", result.StatusCode);
            }
        }

        public async Task<HttpResponseData> InternalServerError(HttpRequestData request, string message)
        {
            return await CreateErrorResponseAsync(request, message, HttpStatusCode.InternalServerError);
        }

        private static void SetJsonContentType(HttpResponseData response)
        {
            if (!response.Headers.Contains("Content-Type"))
            {
                response.Headers.Add("Content-Type", "application/json");
            }
        }
    }
}
