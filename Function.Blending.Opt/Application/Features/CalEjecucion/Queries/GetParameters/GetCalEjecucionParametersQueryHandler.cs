using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Input;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Shared.Results;
using MediatR;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Queries.GetParameters;

public sealed class GetCalEjecucionParametersQueryHandler(
    ICalEjecucionRepository repo,
    ICalInpParameterService parameterService,
    IMapper mapper
  ) : IRequestHandler<GetCalEjecucionParametersQuery, Result<IReadOnlyList<CalInpParametroDto>>>
{
  public async Task<Result<IReadOnlyList<CalInpParametroDto>>> Handle(GetCalEjecucionParametersQuery request, CancellationToken ct)
  {
    var entity = await repo.GetByIdAsync(request.Id, ct);
    if (entity is null)
      return Result<IReadOnlyList<CalInpParametroDto>>.Fail($"Execution '{request.Id}' not found.");

    var rmList = await parameterService.GetParametersByExecutionIdAsync(entity.Id, ct);

    var parametros = mapper.Map<IReadOnlyList<CalInpParametroDto>>(rmList);

    return Result<IReadOnlyList<CalInpParametroDto>>.Ok(parametros);
  }
}