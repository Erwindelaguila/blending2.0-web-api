using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Function.Blending.Auth.Application.Menu.DTOs;
using Function.Blending.Auth.Application.Common.Results;

namespace Function.Blending.Auth.Application.Interfaces.Services
{

    public interface IHttpResponseService
    {

        Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData request, string message, HttpStatusCode statusCode);
        

        Task<HttpResponseData> CreateSuccessResponseAsync<T>(HttpRequestData request, T data, string? message = null);
        
     
        Task<HttpResponseData> ToHttpResponseAsync<T>(HttpRequestData request, Result<T> result);
        
        
        Task<HttpResponseData> InternalServerError(HttpRequestData request, string message);
    }
}
