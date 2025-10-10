namespace Function.Blending.Auth.Application.Interfaces.Services
{
    public interface IAuthorizationHeaderExtractor
    {
        string? ExtractJwtToken(Microsoft.Azure.Functions.Worker.Http.HttpRequestData request);
    }
}
