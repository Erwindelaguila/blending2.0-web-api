using System.Reflection;
using AutoMapper;
using FluentValidation;
using Function.Blending.Opt.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Function.Blending.Opt.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    var asm = Assembly.GetExecutingAssembly();

    // MediatR (commands/queries en Application/*)
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(asm));

    // FluentValidation (validators en el mismo assembly)
    services.AddValidatorsFromAssembly(asm);

    // agrega el pipeline de validación
    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    return services;
  }
}
