using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Cargar config de YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Habilitar CORS para el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Usar YARP
app.MapReverseProxy();

app.Run();