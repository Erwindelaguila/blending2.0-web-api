{
  "IsEncrypted": false,
  "Values": {
    // Configuración básica de Azure Functions
    "AzureWebJobsStorage": "UseDevelopmentStorage=true", // Almacenamiento de desarrollo local
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated", // Worker en .NET aislado
    //Seguridad
    // Credenciales y API Keys
    "SAP_API_KEY": "ST3xF0AL8LfhYZaqyCIaiCcBGGaTBaOF", // API Key para consumir SAP
    // Configuración de Blob Storage en Azure
    "BlobStorage_AccountName": "rgeastustasablending2", // Nombre de la cuenta de almacenamiento
    "BlobStorage_AccountKey": "Yco7THg2MJMIytOTXigvzxo71Nolk1CIvS06HhXhoNWoP+6TaPCDj79drrGvc1uq8tmja6KXLHDp+ASt9MploA==", // Clave de acceso
    "BlobStorage_ContainerName": "exceltasa", // Contenedor donde se guardan los Excel
    // Servicios externos
    "Service_CoreService": "http://localhost:7006", // API Core local
    // Directorio de plantillas
    "Template_Directory": "Templates", // Carpeta donde se guardan las plantillas YAML y Excel
    // Plantillas YAML de entrada
    "Template_ExcelMappingInputSap": "ExcelMappingInputSap.yaml", // Mapeo de datos SAP -> Excel
    "Template_ExcelMappingInputLogistic": "ExcelMappingInputLogistic.yaml", // Mapeo de datos logísticos -> Excel
    "Template_ExcelMappingInputQuality": "ExcelMappingInputQuality.yaml", // Mapeo de datos de calidad -> Excel
    // Plantillas YAML de salida
    "Template_ExcelMappingOutputLogistic": "ExcelMappingOutputLogistic.yaml", // Estructura de salida logística
    "Template_ExcelMappingOutputQuality": "ExcelMappingOutputQuality.yaml", // Estructura de salida calidad
    // Plantillas Excel base para salida
    "Template_LogisticOutput": "TemplateOutputLogistic.xlsx", // Plantilla Excel logística
    "Template_QualityOutput": "TemplateOutputQuality.xlsx", // Plantilla Excel calidad
    "Template_SapOutput": "TemplateOutputSap.xlsx",

    //Segurity
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
    "Auth_Allow_Logistics_ReadById": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Logistics_ReadHistory": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Logistics_WriteStart": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    "Auth_Allow_Logistics_ToggleConfirmed": "c706d3f6-10fa-4a1e-9274-364f60dd3c1f,ee48df9c-0dc1-4428-9bf2-55945b50a6be",
    // === Pipeline flags ===
    "Pipeline_EnableExceptionHandling": "true",
    "Pipeline_EnableRequestLogging": "true",
    "Pipeline_EnableRequestSizeLimit": "true",
    "Pipeline_EnableAuthentication": "true",
    // === Time: Zona Horaria ===
    "Time_TimeZoneId": "America/Lima",
    "Time_WindowsTimeZoneId": "SA Pacific Standard Time",
    "Time_IanaTimeZoneId": "America/Lima"
  },
  "Host": {
    // Configuración de CORS
    "CORS": "*", // Permitir acceso desde cualquier origen
    "CORS_SUPPORT_CREDENTIALS": true // Permitir credenciales en CORS
  }
}