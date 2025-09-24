using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Function.Blending.Auth.Infrastructure.Middleware
{
   
    public class GlobalErrorHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString("N")[..8];
                
                _logger.LogError(ex, 
                    "ERROR_ID: {ErrorId} | Error no controlado en función {FunctionName} | InvocationId: {InvocationId} | Detalles: {ErrorMessage}", 
                    errorId,
                    context.FunctionDefinition.Name, 
                    context.InvocationId,
                    ex.Message);


                if (IsDevelopmentEnvironment())
                {
                    _logger.LogDebug(
                        "ERROR_ID: {ErrorId} | StackTrace completo: {StackTrace}", 
                        errorId, 
                        ex.ToString());
                }

              
                throw;
            }
        }

        private static bool IsDevelopmentEnvironment()
        {
            return string.Equals(
                Environment.GetEnvironmentVariable("Environment"), 
                "Development", 
                StringComparison.OrdinalIgnoreCase);
        }
    }
}