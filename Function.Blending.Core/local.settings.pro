{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "Environment": "Development",
    "SqlConnectionString": "Server=tcp:srv-db-eastus-blending2.database.windows.net,1433;Initial Catalog=db_blending2_prd;Persist Security Info=False;User ID=user_blending2;Password=CXaTHvrMFEZECOF8;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    "AzureAppConfigConnectionString": "Endpoint=https://acblending2.azconfig.io;Id=TLta;Secret=2hVQwWhskJVVJ35Ca9vD1WMXhJE9VlSelq0T7yKpQaAAXMDwRupo…,


    "Auth_DevBypass": "false",
    "Auth_DevGroups": "000000000000000000000000C48B7FA6,4770b5a4-d693-46b2-9e04-eb434d2a3c1b",
    // === Flags Tipos de Validación (Producción solo dejar: Auth_EnableBearerTokens = true ) ===
    "Auth_EnableEasyAuth": "false",
    "Auth_EnableLocalHeaderPrincipal": "false",
    "Auth_EnableHmacPrincipal": "false",
    "Auth_EnableBearerTokens": "true",
    "Auth_Bearer_ValidationMode": "Relaxed", // Relaxed | Strict
    "Auth_Bearer_ValidateLifetime": "false", // en Relaxed puedes poner true|false


    // === ROLES DE NEGOCIO ===
    "Auth_Allow_Administrador": "fb2d63ba-d6c2-4b0f-8bea-8dbbffa51899",
    "Auth_Allow_Calidad": "262c6df9-c3e5-4785-922b-0a9dd8a7ece",
    "Auth_Allow_Logistica": "c0f64f45-72dc-4d8d-a1a8-c225c41a202d",

    // === Pipeline flags ===
    "Pipeline_EnableExceptionHandling": "true",
    "Pipeline_EnableRequestLogging": "true",
    "Pipeline_EnableRequestSizeLimit": "true",
    "Pipeline_EnableAuthentication": "true",
    // === Time: Zona Horaria ===
    "Time_TimeZoneId": "America/Lima",
    "Time_WindowsTimeZoneId": "SA Pacific Standard Time",
    "Time_IanaTimeZoneId": "America/Lima",
    
    //Refecias para estados de Calidad en rowTable
    "Id_Status_Quality" : "98066F2C-9B5D-46E4-A29E-C7D97F8F920C"
    
  },
  "Host": {
    "CORS": "*",
    "CORS_SUPPORT_CREDENTIALS": true
  }
}                    