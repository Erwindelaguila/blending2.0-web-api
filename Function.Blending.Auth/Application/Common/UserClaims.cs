using System.Collections.Generic;

namespace Function.Blending.Auth.Application.Common
{
    public class UserClaims
    {
        public string? ObjectId { get; set; }
        public string? Name { get; set; }
        public List<string> Groups { get; set; } = new();
    }
}