namespace Function.Blending.Auth.Application.Constants
{
    public static class AzureAppConfigKeys
    {
        // Configuración existente para navegación y roles
        public const string AZURE_GROUP_MAPPING = "App-Config-AzureGroupMapping";
        public const string ENLACES = "App-Config-Enlaces";
        public const string NAVIGATION_PERMISOS = "App-Config-Navigation-Permisos";
        
        // Prefijo para configuración de scopes de autorización
        // Formato: Groups:{GroupId}:Scopes
        public const string GROUP_SCOPES_PREFIX = "Groups:";
        public const string GROUP_SCOPES_SUFFIX = ":Scopes";
    }
}