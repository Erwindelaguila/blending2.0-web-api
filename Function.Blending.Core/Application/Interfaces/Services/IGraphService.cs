
namespace Function.Blending.Core.Application.Interfaces.Services
{
    public interface IGraphService
    {
        Task<GraphUserInfo> GetUserInfoAsync(string accessToken);
        Task<List<string>> GetUserGroupsAsync(string accessToken);
    }
    public class GraphUserInfo
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
        public string? Mail { get; set; }
        public string? UserPrincipalName { get; set; }
    }
}
