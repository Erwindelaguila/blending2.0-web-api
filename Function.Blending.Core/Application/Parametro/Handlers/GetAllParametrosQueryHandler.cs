using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class GetAllParametrosQueryHandler : IRequestHandler<GetAllParametrosQuery, PagedResponse<ParametroDTO>>
{
    private readonly IParametroRepository _parametroRepository;

    public GetAllParametrosQueryHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<PagedResponse<ParametroDTO>> Handle(GetAllParametrosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Verificar cancelación temprano
            cancellationToken.ThrowIfCancellationRequested();

            var parametrosQuery = _parametroRepository.GetQueryable();

            if (request.Filters != null)
            {
                parametrosQuery = parametrosQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                parametrosQuery = parametrosQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
                    parametrosQuery = parametrosQuery.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl.Value >= start && x.ModificadoEl.Value < end)
                    );
                }
            }

            parametrosQuery = request.Filters?.Estado switch
            {
                "1" => parametrosQuery.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl),
                "0" => parametrosQuery.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),
                _ => parametrosQuery.OrderBy(x => x.CreadoEl)
            };

            // Proyectar a DTO (hacer antes de paginación para optimizar)
            var parametrosProjected = parametrosQuery.Select(parametro => new ParametroDTO
            {
                Id = parametro.Id,
                Codigo = parametro.Codigo,
                Nombre = parametro.Nombre,
                Descripcion = parametro.Descripcion,
                Activo = parametro.Activo,
                CreadoPorId = parametro.CreadoPorId,
                CreadoEl = parametro.CreadoEl,
                ModificadoPorId = parametro.ModificadoPorId,
                ModificadoEl = parametro.ModificadoEl
            });

            var pagedResult = await parametrosProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    // Orden helper eliminado; lógica inline arriba
}
