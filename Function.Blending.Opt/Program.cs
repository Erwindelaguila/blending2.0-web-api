using AutoMapper;
using Function.Blending.Opt.Application;
using Function.Blending.Opt.Functions.Support.Extensions;   // AddFunctionsSupport + UseFunctionsPipeline
using Function.Blending.Opt.Functions.Support.Security;
using Function.Blending.Opt.Infrastructure;
using Function.Blending.Opt.Infrastructure.Security.KeyDecoders;
using Function.Blending.Opt.Infrastructure.Security.KeyResolvers;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Options.Security;
using Function.Blending.Opt.Shared.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = FunctionsApplication.CreateBuilder(args);

// Integración moderna (middleware/pipeline del worker)
builder.ConfigureFunctionsWebApplication();

// Logging desde configuración
builder.Services.AddLogging(lb =>
  lb.AddConfiguration(builder.Configuration.GetSection(ConfigurationKeys.LoggingSection)));

// Helper para flags desde config
bool on(string key, bool def = true)
  => bool.TryParse(builder.Configuration[key], out var b) ? b : def;

// ====== Services / DI (capas) ======
builder.Services.AddApplication();                         // capa Application
builder.Services.AddInfrastructure(builder.Configuration); // capa Infrastructure (incluye SysParam y AppParam)
builder.Services.AddFunctionsSupport(builder.Configuration); // capa Functions (Support) → ProblemDetails, AuthZ, etc.

// ====== HMAC (Key Vault Resolver + Decoder + Validator) ======
// Nota: este wiring permanece aquí porque no forma parte de tu AddInfrastructure().
builder.Services.Configure<HmacOptions>(opts =>
{
  opts.Resolver = builder.Configuration[ConfigurationKeys.Security.Hmac.Resolver] ?? "KeyVault";
  opts.VaultUrl = builder.Configuration[ConfigurationKeys.Security.Hmac.VaultUrl] ?? string.Empty;
  opts.CacheSeconds = int.TryParse(builder.Configuration[ConfigurationKeys.Security.Hmac.CacheSeconds], out var s) ? s : 600;
  opts.TenantId = builder.Configuration[ConfigurationKeys.Security.Hmac.TenantId];

  opts.Credential = new HmacCredentialOptions
  {
    Mode = builder.Configuration[ConfigurationKeys.Security.Hmac.Credential.Mode] ?? "DefaultNoCli",
    ClientId = builder.Configuration[ConfigurationKeys.Security.Hmac.Credential.ClientId],
    TenantId = builder.Configuration[ConfigurationKeys.Security.Hmac.Credential.TenantId] ?? builder.Configuration[ConfigurationKeys.Security.Hmac.TenantId]
  };
});

builder.Services.AddSingleton<IKeyDecoder, KeyDecoder>();
builder.Services.AddSingleton<IHmacKeyResolver, HmacKeyVaultKeyResolver>();
builder.Services.AddSingleton<IWebhookSignatureValidator, HmacWebhookSignatureValidator>();

// ====== Telemetría ======!
builder.Services
  .AddApplicationInsightsTelemetryWorkerService()
  .ConfigureFunctionsApplicationInsights();

// ====== Pipeline del Worker (ORDEN correcto) ======
// La extensión UseFunctionsPipeline aplica: ContextAccessor, CorrelationId, ExceptionHandling,
// RequestLogging, RequestSizeLimit, HMAC, Principal, Authorization (según flags).
builder.UseFunctionsPipeline(
  enableExceptionHandling: on(ConfigurationKeys.Pipeline.EnableExceptionHandling),
  enableRequestLogging: on(ConfigurationKeys.Pipeline.EnableRequestLogging),
  enableRequestSizeLimit: on(ConfigurationKeys.Pipeline.EnableRequestSizeLimit),
  enableAuthentication: on(ConfigurationKeys.Pipeline.EnableAuthentication)
);

// ====== AutoMapper ======
var assemblies = new[]
{
  typeof(Function.Blending.Opt.Application.DependencyInjection).Assembly,
  typeof(Function.Blending.Opt.Infrastructure.DependencyInjection).Assembly
};
builder.Services.AddAutoMapper(assemblies);

#if DEBUG
using (var sp = builder.Services.BuildServiceProvider())
{
  sp.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
}
#endif

builder.Build().Run();
