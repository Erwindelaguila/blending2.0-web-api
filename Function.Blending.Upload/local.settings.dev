{
  "IsEncrypted": false,
  "Values": {
    // Configuración básica de Azure Functions
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",   // Almacenamiento de desarrollo local
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",         // Worker en .NET aislado

    // Credenciales y API Keys
    "SAP_API_KEY": "ST3xF0AL8LfhYZaqyCIaiCcBGGaTBaOF",     // API Key para consumir SAP

    // Configuración de Blob Storage en Azure
    "BlobStorage_AccountName": "rgeastustasablending2",     // Nombre de la cuenta de almacenamiento
    "BlobStorage_AccountKey": "Yco7THg2MJMIytOTXigvzxo71Nolk1CIvS06HhXhoNWoP+6TaPCDj79drrGvc1uq8tmja6KXLHDp+ASt9MploA==", // Clave de acceso
    "BlobStorage_ContainerName": "exceltasa",              // Contenedor donde se guardan los Excel

    // Servicios externos
    "Service_CoreService": "http://localhost:7006",                // API Core local

    // Directorio de plantillas
    "Template_Directory": "Templates",                     // Carpeta donde se guardan las plantillas YAML y Excel

    // Plantillas YAML de entrada
    "Template_ExcelMappingInputSap": "ExcelMappingInputSap.yaml",           // Mapeo de datos SAP -> Excel
    "Template_ExcelMappingInputLogistic": "ExcelMappingInputLogistic.yaml", // Mapeo de datos logísticos -> Excel
    "Template_ExcelMappingInputQuality": "ExcelMappingInputQuality.yaml",   // Mapeo de datos de calidad -> Excel

    // Plantillas YAML de salida
    "Template_ExcelMappingOutputLogistic": "ExcelMappingOutputLogistic.yaml", // Estructura de salida logística
    "Template_ExcelMappingOutputQuality": "ExcelMappingOutputQuality.yaml",   // Estructura de salida calidad

    // Plantillas Excel base para salida
    "Template_LogisticOutput": "TemplateOutputLogistic.xlsx", // Plantilla Excel logística
    "Template_QualityOutput": "TemplateOutputQuality.xlsx",   // Plantilla Excel calidad
    "Template_SapOutput": "TemplateOutputSap.xlsx"            // Plantilla Excel SAP
  },
  "Host": {
    // Configuración de CORS
    "CORS": "*",                           // Permitir acceso desde cualquier origen
    "CORS_SUPPORT_CREDENTIALS": true       // Permitir credenciales en CORS
  }
}
