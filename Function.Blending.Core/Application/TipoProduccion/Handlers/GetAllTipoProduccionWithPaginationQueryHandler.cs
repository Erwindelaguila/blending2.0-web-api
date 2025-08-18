using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetAllTipoProduccionWithPaginationQueryHandler : IRequestHandler<GetAllTipoProduccionWithPaginationQuery, PagedResponse<TipoProduccionDTO>>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;

    public GetAllTipoProduccionWithPaginationQueryHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<PagedResponse<TipoProduccionDTO>> Handle(GetAllTipoProduccionWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tipoProduccionQuery = _tipoProduccionRepository.GetQueryable();

            if (request.Filters != null)
            {
                tipoProduccionQuery = tipoProduccionQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                tipoProduccionQuery = tipoProduccionQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                tipoProduccionQuery = tipoProduccionQuery.ApplyFechaRangeFilterConTipo(
                    request.Filters.FechaDesde,
                    request.Filters.FechaHasta,
                    request.Filters.TipoFecha,
                    x => x.CreadoEl,
                    x => x.ModificadoEl);
            }

            // Aplicar ordenamiento optimizado
            tipoProduccionQuery = ApplyOptimizedSorting(tipoProduccionQuery, request.Filters?.Estado);

            // Proyectar a DTO (hacer antes de paginación para optimizar)
            var tipoProduccionProjected = tipoProduccionQuery.Select(tipoProduccion => new TipoProduccionDTO
            {
                Id = tipoProduccion.Id,
                Codigo = tipoProduccion.Codigo,
                Nombre = tipoProduccion.Nombre,
                Descripcion = tipoProduccion.Descripcion,
                LineaProduccionId = tipoProduccion.LineaProduccionId,
                AgregadoId = tipoProduccion.AgregadoId,
                Activo = tipoProduccion.Activo,
                CreadoPorId = tipoProduccion.CreadoPorId,
                CreadoEl = tipoProduccion.CreadoEl,
                ModificadoPorId = tipoProduccion.ModificadoPorId,
                ModificadoEl = tipoProduccion.ModificadoEl
            });

            var pagedResult = await tipoProduccionProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    private static IQueryable<TipoProduccionEntity> ApplyOptimizedSorting(IQueryable<TipoProduccionEntity> query, string? estado)
    {
        return estado switch
        {
            "1" => query.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl), // Activos primero
            "0" => query.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),           // Inactivos primero
            _ => query.OrderBy(x => x.CreadoEl)                                    // Por defecto: por fecha
        };
    }
}
