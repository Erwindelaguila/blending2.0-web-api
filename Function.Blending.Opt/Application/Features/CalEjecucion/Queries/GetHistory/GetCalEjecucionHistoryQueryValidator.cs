using FluentValidation;
using System;
using System.Linq;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetHistory;

public sealed class GetCalEjecucionHistoryQueryValidator : AbstractValidator<GetCalEjecucionHistoryQuery>
{
  private static readonly string[] AllowedSortBy = new[] { "creadoEl", "codigo", "estadoId", "plantaId" };

  public GetCalEjecucionHistoryQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThan(0);
    RuleFor(x => x.PageSize).InclusiveBetween(1, 200);

    RuleFor(x => x.SortDir)
      .Must(s => string.IsNullOrWhiteSpace(s) ||
                 s.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                 s.Equals("desc", StringComparison.OrdinalIgnoreCase))
      .WithMessage("sortDir must be 'asc' or 'desc'.");

    RuleFor(x => x.SortBy)
      .Must(s => string.IsNullOrWhiteSpace(s) ||
                 AllowedSortBy.Contains(s.Trim(), StringComparer.OrdinalIgnoreCase))
      .WithMessage($"sortBy must be one of: {string.Join(", ", AllowedSortBy)}.");

    RuleFor(x => x)
      .Must(x => !(x.CreadoDelUtc.HasValue && x.CreadoAlUtc.HasValue) || x.CreadoDelUtc <= x.CreadoAlUtc)
      .WithMessage("creadoDelUtc must be <= creadoAlUtc.");
  }
}
