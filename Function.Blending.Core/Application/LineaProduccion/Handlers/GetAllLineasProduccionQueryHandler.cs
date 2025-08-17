using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class GetAllLineasProduccionQueryHandler : IRequestHandler<GetAllLineasProduccionQuery, PagedResponse<LineaProduccionDTO>>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;

    public GetAllLineasProduccionQueryHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<PagedResponse<LineaProduccionDTO>> Handle(GetAllLineasProduccionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var lineasProduccionQuery = _lineaProduccionRepository.GetQueryable();

            if (request.Filters != null)
            {
                lineasProduccionQuery = lineasProduccionQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                lineasProduccionQuery = lineasProduccionQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                lineasProduccionQuery = lineasProduccionQuery.ApplyFechaRangeFilterConTipo(
                    request.Filters.FechaDesde,
                    request.Filters.FechaHasta,
                    request.Filters.TipoFecha,
                    x => x.CreadoEl,
                    x => x.ModificadoEl);
            }

            // Aplicar ordenamiento optimizado
            lineasProduccionQuery = ApplyOptimizedSorting(lineasProduccionQuery, request.Filters?.Estado);

            // Proyectar a DTO (hacer antes de paginación para optimizar)
            var lineasProduccionProjected = lineasProduccionQuery.Select(lineaProduccion => new LineaProduccionDTO
            {
                Id = lineaProduccion.Id,
                Codigo = lineaProduccion.Codigo,
                Nombre = lineaProduccion.Nombre,
                Descripcion = lineaProduccion.Descripcion,
                Activo = lineaProduccion.Activo,
                CreadoPorId = lineaProduccion.CreadoPorId,
                CreadoEl = lineaProduccion.CreadoEl,
                ModificadoPorId = lineaProduccion.ModificadoPorId,
                ModificadoEl = lineaProduccion.ModificadoEl
            });

            var pagedResult = await lineasProduccionProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    private static IQueryable<LineaProduccionEntity> ApplyOptimizedSorting(IQueryable<LineaProduccionEntity> query, string? estado)
    {
        return estado switch
        {
            "1" => query.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl), // Activos primero
            "0" => query.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),           // Inactivos primero
            _ => query.OrderBy(x => x.CreadoEl)                                    // Por defecto: por fecha
        };
    }
}
