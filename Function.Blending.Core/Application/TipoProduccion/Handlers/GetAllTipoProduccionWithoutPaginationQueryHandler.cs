using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetAllTipoProduccionWithoutPaginationQueryHandler : IRequestHandler<GetAllTipoProduccionWithoutPaginationQuery, List<TipoProduccionDTO>>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;

    public GetAllTipoProduccionWithoutPaginationQueryHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<List<TipoProduccionDTO>> Handle(GetAllTipoProduccionWithoutPaginationQuery request, CancellationToken cancellationToken)
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

            // Ordenar por código por defecto
            return allTipoProduccion.OrderBy(x => x.CreadoEl).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los tipos de producción sin paginación", ex);
        }
    }
}
