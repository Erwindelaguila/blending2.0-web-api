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
}
