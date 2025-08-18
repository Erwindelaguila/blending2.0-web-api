using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class GetAllPlantasWithPaginationQueryHandler : IRequestHandler<GetAllPlantasWithPaginationQuery, PagedResponse<PlantaDTO>>
{
    private readonly IPlantaRepository _plantaRepository;

    public GetAllPlantasWithPaginationQueryHandler(IPlantaRepository plantaRepository)
    {
        _plantaRepository = plantaRepository;
    }

    public async Task<PagedResponse<PlantaDTO>> Handle(GetAllPlantasWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var plantasQuery = _plantaRepository.GetQueryable();

            if (request.Filters != null)
            {
                plantasQuery = plantasQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                plantasQuery = plantasQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                plantasQuery = plantasQuery.ApplyFechaRangeFilterConTipo(
                    request.Filters.FechaDesde,
                    request.Filters.FechaHasta,
                    request.Filters.TipoFecha,
                    x => x.CreadoEl,
                    x => x.ModificadoEl);
            }

            // Aplicar ordenamiento optimizado
            plantasQuery = ApplyOptimizedSorting(plantasQuery, request.Filters?.Estado);

            // Proyectar a DTO (hacer antes de paginación para optimizar)
            var plantasProjected = plantasQuery.Select(planta => new PlantaDTO
            {
                Id = planta.Id,
                Codigo = planta.Codigo,
                Nombre = planta.Nombre,
                Descripcion = planta.Descripcion,
                Activo = planta.Activo,
                CreadoPorId = planta.CreadoPorId,
                CreadoEl = planta.CreadoEl,
                ModificadoPorId = planta.ModificadoPorId,
                ModificadoEl = planta.ModificadoEl
            });

            var pagedResult = await plantasProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    private static IQueryable<PlantaEntity> ApplyOptimizedSorting(IQueryable<PlantaEntity> query, string? estado)
    {
        return estado switch
        {
            "1" => query.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl), // Activos primero
            "0" => query.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),           // Inactivos primero
            _ => query.OrderBy(x => x.CreadoEl)                                    // Por defecto: por fecha
        };
    }
}
