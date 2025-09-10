{
  "IsEncrypted": false,
  "ConnectionStrings": {
    "PROTECSO_BlendingDb": "Server=tcp:blending-svr-new.database.windows.net,1433;Initial Catalog=blending-dev;Persist Security Info=False;User ID=blending-usr;Password=bl3nd1ng-pwD!.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "BlendingDb": "Server=tcp:srv-db-eastus-blending2.database.windows.net,1433;Initial Catalog=db_blending2_prd;Persist Security Info=False;User ID=user_blending2;Password=CXaTHvrMFEZECOF8;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  },
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    // === Auth (CSV) ===
    "Auth:DevBypass": "false",
    "Auth:DevGroups": "000000000000000000000000C48B7FA6,4770b5a4-d693-46b2-9e04-eb434d2a3c1b",

    // === Flags Tipos de Validación (Producción solo dejar: Auth:EnableBearerTokens = true ) ===
    "Auth:EnableEasyAuth": "false",
    "Auth:EnableLocalHeaderPrincipal": "true",
    "Auth:EnableHmacPrincipal": "false",
    "Auth:EnableBearerTokens": "true",
    "Auth:Bearer:ValidationMode": "Relaxed", // Relaxed | Strict
    "Auth:Bearer:ValidateLifetime": "false", // en Relaxed puedes poner true|false
    "Auth:Bearer:TenantId": "fc810a1e-f6b2-40b7-96a1-32abada72fd8",
    "Auth:Bearer:Authority": "https://login.microsoftonline.com/fc810a1e-f6b2-40b7-96a1-32abada72fd8/v2.0",
    "Auth:Bearer:Audience": "api://f8d7cce6-0cf4-46cf-a13d-66f1099c05c8",
    "Auth:Bearer:ClockSkewSeconds": "300",

    // === Scopes dinámicos (CSV) ===
    "Auth:Allow:Quality:ReadById": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth:Allow:Quality:ReadHistory": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth:Allow:Quality:WriteStart": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth:Allow:Quality:ToggleState": "ee48df9c-0dc1-4428-9bf2-55945b50a6be",

    "Auth:Allow:Logistics:ReadById": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth:Allow:Logistics:ReadHistory": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth:Allow:Logistics:WriteStart": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth:Allow:Logistics:ToggleConfirmed": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",

    // === Límites ===
    "Limits:WebhookMaxBytes": "2097152",

    // === Catálogo AuxTable por Id ===
    "Catalog:QualityExecutionStatus:Id": "99F7B28F-4CD5-4EC7-81A3-AFF474332949",
    "Catalog:LogisticExecutionStatus:Id": "4E38D655-602E-452B-8A06-1A014247342B",

    // === Catálogo AuxProp por Clave ===
    "Catalog:QualityExecutionStatus:Prop:ExposeColor": "true",
    "Catalog:QualityExecutionStatus:Prop:ColorClave": "color",
    "Catalog:LogisticExecutionStatus:Prop:ExposeColor": "true",
    "Catalog:LogisticExecutionStatus:Prop:ColorClave": "color",


    // === Caché de Estados de Calidad ===
    "Catalog:QualityExecutionStatus:Cache:Enabled": "true",
    "Catalog:QualityExecutionStatus:Cache:TtlSeconds": "300",
    "Catalog:QualityExecutionStatus:Cache:CacheNulls": "false",
    "Catalog:LogisticExecutionStatus:Cache:Enabled": "true",
    "Catalog:LogisticExecutionStatus:Cache:TtlSeconds": "300",
    "Catalog:LogisticExecutionStatus:Cache:CacheNulls": "false",

    // === Catálogo AuxRow por Id ===
    "Catalog:QualityExecutionStatus:EnEjecucion": "08AB1FA5-FA0D-4BB8-8A89-453C2D4897BF",
    "Catalog:QualityExecutionStatus:Procesado": "5124313F-7DCB-47A0-B84B-5833E383231E",
    "Catalog:QualityExecutionStatus:Aceptado": "D606881A-4262-4AE0-BAF9-397F538DC710",
    "Catalog:QualityExecutionStatus:Cancelado": "AC41946B-D7FA-44F1-A04A-78432385AB9B",
    "Catalog:QualityExecutionStatus:Error": "C3BA3FB5-E16B-4485-B4EB-89ADAFFF2771",

    "Catalog:LogisticExecutionStatus:EnEjecucion": "96876821-E29B-449B-B034-1BC3DEDC47C1",
    "Catalog:LogisticExecutionStatus:Procesado": "C4C222F9-F83A-4686-9E35-0871282D3CD3",
    "Catalog:LogisticExecutionStatus:Cancelado": "AAAF66C1-83DC-470B-9CD1-9A7410CA4734",
    "Catalog:LogisticExecutionStatus:Error": "E36A9812-0D06-4830-8FA5-191375C427F1",

    // === AppParam Keys ===
    "AppParam:QualityExecutionCodeFormat": "APP_CAL_CODIGO_FORMAT",
    "AppParam:LogisticExecutionCodeFormat": "APP_LOG_CODIGO_FORMAT",

    // === Chaché de AppParam ===
    "AppParamCache:Enabled": "true",
    "AppParamCache:DefaultTtlSeconds": "300",
    "AppParamCache:CacheNulls": "false",
    "AppParamCache:PerKeyTtlSeconds:APP_CAL_CODIGO_FORMAT": "1800",
    "AppParamCache:PerKeyTtlSeconds:APP_LOG_CODIGO_FORMAT": "1800",

    // === SysParam Keys ===
    "SysParam:SystemUser": "SYS_USUARIO_SISTEMA",

    // === Caché de SysParam ===
    "SysParamCache:Enabled": "true",
    "SysParamCache:DefaultTtlSeconds": "300",
    "SysParamCache:CacheNulls": "false",
    "SysParamCache:PerKeyTtlSeconds:SYS_USUARIO_SISTEMA": "3600",

    // === Defaults ===
    "Defaults:Quality:Execution:Format": "CAL{0:D6}",
    "Defaults:Logistic:Execution:Format": "LOG{0:D6}",

    // === Logging ===
    "Logging:LogLevel:Default": "Debug", // cambia a "Warning" para menos ruido, "Debug"/"Trace" para máximo detalle
    "Logging:LogLevel:Microsoft": "Warning", // baja ruido de framework
    "Logging:LogLevel:Microsoft.Azure.Functions.Worker": "Information",

    // === Pipeline flags ===
    "Pipeline:EnableExceptionHandling": "true",
    "Pipeline:EnableRequestLogging": "true",
    "Pipeline:EnableRequestSizeLimit": "true",
    "Pipeline:EnableAuthentication": "true",

    // === Time: Zona Horaria ===
    "Time:TimeZoneId": "America/Lima",
    "Time:WindowsTimeZoneId": "SA Pacific Standard Time",
    "Time:IanaTimeZoneId": "America/Lima",

    // === Security ===
    "Security:Hmac:Resolver": "KeyVault",
    "Security:Hmac:VaultUrl": "https://kvblending2.vault.azure.net/",
    "Security:Hmac:CacheSeconds": "600",
    "Security:Hmac:TenantId": "b7e26f48-2292-4a14-a355-1aeb8489ae3d", // opcional, ayuda en multi-tenant
    "Security:Hmac:Credential:Mode": "Interactive" // usa CLI si existe; si no, abre login interactivo
  }
}
