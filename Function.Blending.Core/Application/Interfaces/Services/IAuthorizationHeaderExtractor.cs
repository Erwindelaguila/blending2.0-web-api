using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.Interfaces.Services;

public interface IAuthorizationHeaderExtractor
{
    string? ExtractJwtToken(HttpRequestData request);
}
