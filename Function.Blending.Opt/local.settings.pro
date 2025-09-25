{
  "IsEncrypted": false,
  "ConnectionStrings": {
    "BlendingDb": "Server=tcp:srv-db-eastus-blending2.database.windows.net,1433;Initial Catalog=db_blending2_prd;Persist Security Info=False;User ID=user_blending2;Password=CXaTHvrMFEZECOF8;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    // === DDBB ===
    "SqlConnectionString_BlendingDb": "Server=tcp:srv-db-eastus-blending2.database.windows.net,1433;Initial Catalog=db_blending2_prd;Persist Security Info=False;User ID=user_blending2;Password=CXaTHvrMFEZECOF8;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",

    // === Auth (CSV) ===
    "Auth_DevBypass": "false",
    "Auth_DevGroups": "000000000000000000000000C48B7FA6,4770b5a4-d693-46b2-9e04-eb434d2a3c1b",

    // === Flags Tipos de Validación (Producción solo dejar: Auth_EnableBearerTokens = true ) ===
    "Auth_EnableEasyAuth": "false",
    "Auth_EnableLocalHeaderPrincipal": "false",
    "Auth_EnableHmacPrincipal": "false",
    "Auth_EnableBearerTokens": "true",
    "Auth_Bearer_ValidationMode": "Relaxed", // Relaxed | Strict
    "Auth_Bearer_ValidateLifetime": "false", // en Relaxed puedes poner true|false
    "Auth_Bearer_TenantId": "fc810a1e-f6b2-40b7-96a1-32abada72fd8",
    "Auth_Bearer_Authority": "https://login.microsoftonline.com/fc810a1e-f6b2-40b7-96a1-32abada72fd8/v2.0",
    "Auth_Bearer_Audience": "api://f8d7cce6-0cf4-46cf-a13d-66f1099c05c8",
    "Auth_Bearer_ClockSkewSeconds": "300",

    // === Scopes dinámicos (CSV) ===
    "Auth_Allow_Quality_ReadById": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Quality_ReadHistory": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Quality_WriteStart": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Quality_ChangeAccepted": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Quality_ReadParameters": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Quality_ReadInputById": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Quality_ReadOutputById": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",

    "Auth_Allow_Logistics_ReadById": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Logistics_ReadHistory": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Logistics_WriteStart": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Logistics_ToggleConfirmed": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",

    // === Límites ===
    "Limits_WebhookMaxBytes": "2097152",

    // === Catálogo AuxTable por Id ===
    "Catalog_QualityExecutionStatus_Id": "98066F2C-9B5D-46E4-A29E-C7D97F8F920C",
    "Catalog_LogisticExecutionStatus_Id": "4E38D655-602E-452B-8A06-1A014247342B",

    // === Catálogo AuxProp por Clave ===
    "Catalog_QualityExecutionStatus_Prop_ExposeColor": "true",
    "Catalog_QualityExecutionStatus_Prop_ColorClave": "color",
    "Catalog_LogisticExecutionStatus_Prop_ExposeColor": "true",
    "Catalog_LogisticExecutionStatus_Prop_ColorClave": "color",

    // === Caché de Estados de Calidad ===
    "Catalog_QualityExecutionStatus_Cache_Enabled": "true",
    "Catalog_QualityExecutionStatus_Cache_TtlSeconds": "300",
    "Catalog_QualityExecutionStatus_Cache_CacheNulls": "false",
    "Catalog_LogisticExecutionStatus_Cache_Enabled": "true",
    "Catalog_LogisticExecutionStatus_Cache_TtlSeconds": "300",
    "Catalog_LogisticExecutionStatus_Cache_CacheNulls": "false",

    // === Catálogo AuxRow por Id ===
    "Catalog_QualityExecutionStatus_EnEjecucion": "37AFC095-E1FA-41A6-8140-15BB04CEB0EF",
    "Catalog_QualityExecutionStatus_Procesado": "D86B3EF4-1331-4FA7-97E4-4ADB1518AE6E",
    "Catalog_QualityExecutionStatus_Cancelado": "2F35D633-39B7-4C7F-9105-1FF25B8CFA72",
    "Catalog_QualityExecutionStatus_Error": "AFC50EAF-264D-42DA-A203-B9E093ECC625",

    "Catalog_LogisticExecutionStatus_EnEjecucion": "96876821-E29B-449B-B034-1BC3DEDC47C1",
    "Catalog_LogisticExecutionStatus_Procesado": "C4C222F9-F83A-4686-9E35-0871282D3CD3",
    "Catalog_LogisticExecutionStatus_Cancelado": "AAAF66C1-83DC-470B-9CD1-9A7410CA4734",
    "Catalog_LogisticExecutionStatus_Error": "E36A9812-0D06-4830-8FA5-191375C427F1",

    // === AppParam Keys ===
    "AppParam_QualityExecutionCodeFormat": "APP_CAL_CODIGO_FORMAT",
    "AppParam_LogisticExecutionCodeFormat": "APP_LOG_CODIGO_FORMAT",

    // === Chaché de AppParam ===
    "AppParamCache_Enabled": "true",
    "AppParamCache_DefaultTtlSeconds": "300",
    "AppParamCache_CacheNulls": "false",
    "AppParamCache_PerKeyTtlSeconds_APP_CAL_CODIGO_FORMAT": "1800",
    "AppParamCache_PerKeyTtlSeconds_APP_LOG_CODIGO_FORMAT": "1800",

    // === SysParam Keys ===
    "SysParam_SystemUser": "SYS_USUARIO_SISTEMA",

    // === Caché de SysParam ===
    "SysParamCache_Enabled": "true",
    "SysParamCache_DefaultTtlSeconds": "300",
    "SysParamCache_CacheNulls": "false",
    "SysParamCache_PerKeyTtlSeconds_SYS_USUARIO_SISTEMA": "3600",

    // === Defaults ===
    "Defaults_Quality_Execution_Format": "CAL{0:D6}",
    "Defaults_Logistic_Execution_Format": "LOG{0:D6}",

    // === Logging ===
    "Logging_LogLevel_Default": "Debug", // cambia a "Warning" para menos ruido, "Debug"/"Trace" para máximo detalle
    "Logging_LogLevel_Microsoft": "Warning", // baja ruido de framework
    "Logging_LogLevel_Microsoft.Azure.Functions.Worker": "Information",

    // === Pipeline flags ===
    "Pipeline_EnableExceptionHandling": "true",
    "Pipeline_EnableRequestLogging": "true",
    "Pipeline_EnableRequestSizeLimit": "true",
    "Pipeline_EnableAuthentication": "true",

    // === Time: Zona Horaria ===
    "Time_TimeZoneId": "America/Lima",
    "Time_WindowsTimeZoneId": "SA Pacific Standard Time",
    "Time_IanaTimeZoneId": "America/Lima",

    // === Security ===
    "Security_Hmac_Resolver": "KeyVault",
    "Security_Hmac_VaultUrl": "https://kvblending2.vault.azure.net/",
    "Security_Hmac_CacheSeconds": "600",
    "Security_Hmac_TenantId": "b7e26f48-2292-4a14-a355-1aeb8489ae3d", // opcional, ayuda en multi-tenant
    "Security_Hmac_Credential_Mode": "Interactive", // usa CLI si existe; si no, abre login interactivo -> PROD: ManagedIdentity -> necesario(Security_Hmac_Credential_ClientId)
    "Security_Hmac_Credential_ClientId": "b7e26f48-2292-4a14-a355-1aeb8489ae3d", // <clientId-de-la-UAMI>
    "Security_Hmac_Credential_TenantId": "b7e26f48-2292-4a14-a355-1aeb8489ae3d",

    // ===== External - Calidad Model =====
    //"External_QualityModel_BaseUrl": "https://app-blending2-modelo-calidad-prod.azurewebsites.net",
    "External_QualityModel_BaseUrl": "http://localhost:5000",
    "External_QualityModel_StartPath": "/blending_harina",
    "External_QualityModel_TimeoutSeconds": "600",
    "External_QualityModel_ApiKey": null,

    // ===== Logging DB =====
    "Logging_Db_Enabled": "true",
    "Logging_Db_MinLevel": "error", // reservado (no usado ahora)
    "Logging_Db_SaveInfo": "false", // <= por defecto apagado
    "Logging_Db_Problem4xxAs": "warning" // nivel para ProblemDetails 4xx
  }
}
