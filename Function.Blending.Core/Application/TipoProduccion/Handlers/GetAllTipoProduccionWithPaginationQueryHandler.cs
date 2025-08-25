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
            // Obtener todos los datos con estructura anidada
            var allTipoProduccion = await _tipoProduccionRepository.GetAllWithRelationsAsync();
            
            // Aplicar filtros en memoria usando request.Filters
            if (request.Filters != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Filters.Codigo))
                    allTipoProduccion = allTipoProduccion.Where(x => x.Codigo.Contains(request.Filters.Codigo, StringComparison.OrdinalIgnoreCase)).ToList();
                
                if (!string.IsNullOrWhiteSpace(request.Filters.Estado))
                {
                    var isActivo = request.Filters.Estado == "1" || request.Filters.Estado.ToLower() == "activo";
                    allTipoProduccion = allTipoProduccion.Where(x => x.Activo == isActivo).ToList();
                }
                
                if (request.Filters.FechaDesde.HasValue)
                    allTipoProduccion = allTipoProduccion.Where(x => x.CreadoEl >= request.Filters.FechaDesde.Value).ToList();
            }

            // Ordenar por fecha de creación (más recientes al final)
            allTipoProduccion = allTipoProduccion.OrderBy(x => x.CreadoEl).ToList();

            // Aplicar paginación
            var totalRecords = allTipoProduccion.Count;
            var tipoProduccionPaginated = allTipoProduccion
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .ToList();

            return new PagedResponse<TipoProduccionDTO>
            {
                Items = tipoProduccionPaginated,
                Pagination = new PaginationInfo
                {
                    CurrentPage = request.Page,
                    PageSize = request.Size,
                    TotalCount = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / request.Size),
                    HasPrevious = request.Page > 1,
                    HasNext = request.Page < (int)Math.Ceiling((double)totalRecords / request.Size),
                    PreviousPage = request.Page > 1 ? request.Page - 1 : null,
                    NextPage = request.Page < (int)Math.Ceiling((double)totalRecords / request.Size) ? request.Page + 1 : null
                }
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los tipos de producción paginados", ex);
        }
    }

    // Orden helper eliminado; lógica inline arriba
}
