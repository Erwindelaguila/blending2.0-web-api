namespace Function.Blending.Core.Application.Constants
{
    /// <summary>
    /// Constantes para las claves de Azure App Configuration.
    /// Estas claves definen los nombres de configuración utilizados en Azure App Configuration.
    /// </summary>
    public static class AzureAppConfigKeys
    {
        /// <summary>
        /// Clave para el mapeo de grupos de Azure AD a roles internos del sistema.
        /// Formato: {"grupoId1": "rol1", "grupoId2": "rol2"}
        /// </summary>
        public const string AZURE_GROUP_MAPPING = "App-Config-AzureGroupMapping";

        /// <summary>
        /// Clave para la configuración de enlaces de navegación.
        /// Contiene la definición de todos los enlaces disponibles en el sistema.
        /// </summary>
        public const string ENLACES = "App-Config-Enlaces";

        /// <summary>
        /// Clave para los permisos de navegación por rol.
        /// Define qué enlaces puede acceder cada rol del sistema.
        /// </summary>
        public const string NAVIGATION_PERMISOS = "App-Config-Navigation-Permisos";
    }

    /// <summary>
    /// Constantes para Microsoft Graph API.
    /// </summary>
    public static class GraphApiConstants
    {
        /// <summary>
        /// URL base de Microsoft Graph API v1.0.
        /// </summary>
        public const string BASE_URL = "https://graph.microsoft.com/v1.0";
        
        /// <summary>
        /// Audience ID para Microsoft Graph en tokens de Azure AD.
        /// </summary>
        public const string MICROSOFT_GRAPH_AUDIENCE = "00000003-0000-0000-c000-000000000000";
    }

    /// <summary>
    /// Constantes para Azure AD y autenticación.
    /// </summary>
    public static class AzureAuthConstants
    {
        /// <summary>
        /// URL base para obtener las claves públicas de Azure AD.
        /// </summary>
        public const string AZURE_AD_KEYS_URL = "https://login.microsoftonline.com/common/discovery/keys";
    }
}
