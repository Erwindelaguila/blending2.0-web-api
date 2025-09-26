using System;
using Function.Blending.Auth.Application.Interfaces.Services;

namespace Function.Blending.Auth.Infrastructure.Services
{
    public class TokenConfigurationService : ITokenConfigurationService
    {
        public bool IsDevelopmentMode { get; }

        public TokenConfigurationService()
        {
            IsDevelopmentMode = Environment.GetEnvironmentVariable("Environment") == "Development";
        }
    }
}
