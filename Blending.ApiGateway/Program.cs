using Yarp.ReverseProxy;
using Blending.ApiGateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

/*
 * CONFIGURACIÓN DEL API GATEWAY PARA SIMULAR AZURE API MANAGEMENT
 * 
 * PROPÓSITO:
 * - En DESARROLLO: Este gateway simula exactamente el comportamiento de Azure APIM
 * - En PRODUCCIÓN: Azure APIM real ejecuta las mismas operaciones automáticamente
 * 
 * GARANTÍA: 
 * - Las Azure Functions reciben headers idénticos en desarrollo y producción
 * - No hay diferencias de código entre ambientes
 * - La migración a APIM es transparente
 */

// Configurar YARP (Yet Another Reverse Proxy) para desarrollo únicamente
// En producción, Azure APIM reemplaza completamente a YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Configurar CORS para permitir requests del frontend React
// APIM en producción maneja CORS automáticamente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()      // Permitir cualquier origen
              .AllowAnyMethod()      // Permitir GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader();     // Permitir Authorization, Content-Type, etc.
    });
});

var app = builder.Build();

/*
 * PIPELINE DE MIDDLEWARE - ORDEN CRÍTICO
 * 
 * 1. CORS primero: Maneja requests OPTIONS (preflight) sin autenticación
 * 2. APIM Simulator: Valida JWT y agrega headers X-User-*
 * 3. YARP: Hace proxy a las Azure Functions
 */

// PASO 1: Habilitar CORS para requests del navegador
// Debe ir ANTES del middleware de autenticación para manejar preflight
app.UseCors("AllowFrontend");

// PASO 2: Middleware que simula Azure APIM
// Valida tokens JWT y agrega headers de usuario exactamente como APIM
app.UseMiddleware<ApimSimulatorMiddleware>();

// PASO 3: Proxy reverso a las Azure Functions
// YARP enruta requests a localhost:7056 (Auth), localhost:7006 (Core), etc.
app.MapReverseProxy();

app.Run();