using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class GetAllParametrosWithPaginationQueryHandler : IRequestHandler<GetAllParametrosWithPaginationQuery, PagedResponse<ParametroDTO>>
{
    private readonly IParametroRepository _parametroRepository;

    public GetAllParametrosWithPaginationQueryHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<PagedResponse<ParametroDTO>> Handle(GetAllParametrosWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var parametrosQuery = _parametroRepository.GetQueryable();

            if (request.Filters != null)
            {
                parametrosQuery = parametrosQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                parametrosQuery = parametrosQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                parametrosQuery = parametrosQuery.ApplyFechaRangeFilterConTipo(
                    request.Filters.FechaDesde,
                    request.Filters.FechaHasta,
                    request.Filters.TipoFecha,
                    x => x.CreadoEl,
                    x => x.ModificadoEl);
            }

            // Aplicar ordenamiento optimizado
            parametrosQuery = ApplyOptimizedSorting(parametrosQuery, request.Filters?.Estado);

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

    private static IQueryable<ParametroEntity> ApplyOptimizedSorting(IQueryable<ParametroEntity> query, string? estado)
    {
        return estado switch
        {
            "1" => query.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl), // Activos primero
            "0" => query.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),           // Inactivos primero
            _ => query.OrderBy(x => x.CreadoEl)                                    // Por defecto: por fecha
        };
    }
}
