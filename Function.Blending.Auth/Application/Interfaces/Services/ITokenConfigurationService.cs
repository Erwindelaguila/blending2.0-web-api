namespace Function.Blending.Auth.Application.Interfaces.Services
{
    public interface ITokenConfigurationService
    {
        bool IsDevelopmentMode { get; }
        string TenantId { get; }
        string ExpectedClientId { get; }
        List<string> AllowedClientIds { get; }
        string Authority { get; }
    }
}
