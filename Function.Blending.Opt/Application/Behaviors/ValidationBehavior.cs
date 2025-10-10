using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Behaviors;

// Pipeline que corre TODOS los IValidator<TRequest> antes del handler.
// Si hay errores, lanza FluentValidation.ValidationException (fail-fast).
public sealed class ValidationBehavior<TRequest, TResponse>(
  IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
  where TRequest : notnull
{
  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken ct)
  {
    if (!validators.Any())
      return await next();

    var ctx = new ValidationContext<TRequest>(request);
    var failures = new List<ValidationFailure>();

    foreach (var v in validators)
    {
      var result = await v.ValidateAsync(ctx, ct);
      if (!result.IsValid)
        failures.AddRange(result.Errors);
    }

    if (failures.Count > 0)
      throw new ValidationException(failures);

    return await next();
  }
}
