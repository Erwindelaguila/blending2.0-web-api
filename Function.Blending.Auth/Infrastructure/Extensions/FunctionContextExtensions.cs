using System.Security.Claims;
using Function.Blending.Auth.Application.Common;
using Microsoft.Azure.Functions.Worker;
using Function.Blending.Auth.Infrastructure.Middleware;

namespace Function.Blending.Auth.Infrastructure.Extensions
{
    public static class FunctionContextExtensions
    {
        private const string UserClaimsKey = "UserClaims";
        private const string JwtTokenKey = "JwtToken";

        public static UserClaims? GetUserClaims(this FunctionContext context)
        {
            return context.Items.TryGetValue(UserClaimsKey, out var claims) 
                ? claims as UserClaims 
                : null;
        }

        public static string? GetJwtToken(this FunctionContext context)
        {
            return context.Items.TryGetValue(JwtTokenKey, out var token) 
                ? token as string 
                : null;
        }

        public static bool IsUserAuthenticated(this FunctionContext context)
        {
            return context.GetUserClaims() != null;
        }
    }
}