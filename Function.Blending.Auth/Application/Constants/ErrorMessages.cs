namespace Function.Blending.Auth.Application.Constants
{

    public static class ErrorMessages
    {
        // Mensajes de autenticación
        public const string TokenNotProvided = "AUTH_001: Token no proporcionado";
        public const string TokenInvalid = "AUTH_002: Token inválido o no autorizado";
        public const string TokenExpired = "AUTH_003: El token JWT ha expirado";
        public const string TokenNotYetValid = "AUTH_004: El token JWT aún no es válido";
        public const string TokenDeserializationFailed = "AUTH_005: El token no es válido o no se puede deserializar";
        public const string UserNotAuthenticated = "AUTH_006: Usuario no autenticado";
        
        // Mensajes de autorización
        public const string InsufficientPermissions = "AUTHZ_001: Permisos insuficientes para realizar esta acción";
        public const string InvalidUserClaims = "AUTHZ_002: Claims de usuario inválidos";
        public const string RoleNotFound = "AUTHZ_003: Rol de usuario no encontrado";
        public const string GroupsNotFound = "AUTHZ_004: Grupos de usuario no encontrados";
        
        // Mensajes de errores internos
        public const string InternalServerError = "SRV_001: Error interno del servidor";
        public const string ValidationError = "VAL_001: Error durante la validación del token";
        public const string AuthenticationError = "ERR_001: Error interno de autenticación";
        public const string ConfigurationError = "CFG_001: Error de configuración del servicio";
        
        // Mensajes de headers
        public const string AuthorizationHeaderNotFound = "HDR_001: Header Authorization no encontrado";
        public const string InvalidAuthorizationHeaderFormat = "HDR_002: Formato de Authorization header inválido";

        // Mensajes de menú/funcionalidad
        public const string MenuNotFound = "MENU_001: Menú no encontrado para el usuario";
        public const string MenuAccessDenied = "MENU_002: Acceso denegado al menú solicitado";
        
        // Mensajes de validación de negocio
        public const string InvalidRequestData = "REQ_001: Datos de solicitud inválidos";
        public const string RequiredFieldMissing = "REQ_002: Campo requerido faltante";
    }
}