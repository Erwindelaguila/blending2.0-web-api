using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class GetAllAgregadosQueryHandler : IRequestHandler<GetAllAgregadosQuery, PagedResponse<AgregadoDTO>>
{
    private readonly IAgregadoRepository _agregadoRepository;

    public GetAllAgregadosQueryHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }

    public async Task<PagedResponse<AgregadoDTO>> Handle(GetAllAgregadosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var agregadosQuery = _agregadoRepository.GetQueryable();

            if (request.Filters != null)
            {
                agregadosQuery = agregadosQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                agregadosQuery = agregadosQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                agregadosQuery = agregadosQuery.ApplyFechaRangeFilterConTipo(
                    request.Filters.FechaDesde,
                    request.Filters.FechaHasta,
                    request.Filters.TipoFecha,
                    x => x.CreadoEl,
                    x => x.ModificadoEl);
            }

            // Aplicar ordenamiento optimizado
            agregadosQuery = ApplyOptimizedSorting(agregadosQuery, request.Filters?.Estado);

            // Proyectar a DTO (hacer antes de paginación para optimizar)
            var agregadosProjected = agregadosQuery.Select(agregado => new AgregadoDTO
            {
                Id = agregado.Id,
                Codigo = agregado.Codigo,
                Nombre = agregado.Nombre,
                Descripcion = agregado.Descripcion,
                Activo = agregado.Activo,
                CreadoPorId = agregado.CreadoPorId,
                CreadoEl = agregado.CreadoEl,
                ModificadoPorId = agregado.ModificadoPorId,
                ModificadoEl = agregado.ModificadoEl
            });

            var pagedResult = await agregadosProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    private static IQueryable<AgregadoEntity> ApplyOptimizedSorting(IQueryable<AgregadoEntity> query, string? estado)
    {
        return estado switch
        {
            "1" => query.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl), // Activos primero
            "0" => query.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),           // Inactivos primero
            _ => query.OrderBy(x => x.CreadoEl)                                    // Por defecto: por fecha
        };
    }
}
