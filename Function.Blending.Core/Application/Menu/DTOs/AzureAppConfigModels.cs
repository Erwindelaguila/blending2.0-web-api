
namespace Function.Blending.Core.Application.Menu.DTOs
{
    public class AzureAppConfigModels
    {
        public class AzureGroupMapping
        {
            public Dictionary<string, string>? GroupToRoleMap { get; set; }
        }

        public class EnlacesConfiguration
        {
            public Dictionary<string, EnlaceItem>? Enlaces { get; set; }
        }

        public class NavigationPermisos
        {
            public Dictionary<string, List<string>>? PermisosPorRol { get; set; }
        }
    }
}
