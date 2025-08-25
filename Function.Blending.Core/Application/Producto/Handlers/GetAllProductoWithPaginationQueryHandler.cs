using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Producto.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class GetAllProductoWithPaginationQueryHandler : IRequestHandler<GetAllProductoWithPaginationQuery, PagedResponse<ProductoDTO>>
{
    private readonly IProductoRepository _productoRepository;

    public GetAllProductoWithPaginationQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<PagedResponse<ProductoDTO>> Handle(GetAllProductoWithPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Usar el IQueryable del modelo EF directamente para poder aplicar Include antes de proyectar
            var productoEntityQuery = _productoRepository.GetEntityQueryable();

            // Aplicar filtros si existen
            if (request.Filters != null)
            {
                productoEntityQuery = productoEntityQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                productoEntityQuery = productoEntityQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
            productoEntityQuery = productoEntityQuery.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl.Value >= start && x.ModificadoEl.Value < end)
                    );
                }

                // Filtro por CalidadId
                if (request.Filters.CalidadId.HasValue)
                {
                    productoEntityQuery = productoEntityQuery.Where(x => x.CalidadId == request.Filters.CalidadId.Value);
                }

                // Filtro por TipoProduccionId  
                if (request.Filters.TipoProduccionId.HasValue)
                {
                    productoEntityQuery = productoEntityQuery.Where(x => x.TipoProduccionId == request.Filters.TipoProduccionId.Value);
                }
            }

            // Contar registros ANTES de cualquier proyección o Include
        var totalCount = await productoEntityQuery.CountAsync(cancellationToken);
            
            if (totalCount == 0)
            {
                return new PagedResponse<ProductoDTO>
                {
                    Items = new List<ProductoDTO>(),
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = request.Page,
                        TotalPages = 0,
                        PageSize = request.Size,
                        TotalCount = 0,
                        HasPrevious = false,
                        HasNext = false,
                        PreviousPage = null,
                        NextPage = null
                    }
                };
            }

            // Aplicar Include ANTES de proyección
            productoEntityQuery = productoEntityQuery
                .Include(p => p.Calidad)
                .Include(p => p.TipoProduccion);

            // Orden: por estado si viene, si no por CreadoEl
            productoEntityQuery = request.Filters?.Estado switch
            {
                "1" => productoEntityQuery.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl),
                "0" => productoEntityQuery.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),
                _ => productoEntityQuery.OrderBy(x => x.CreadoEl)
            };

            // Calcular paginación
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.Size);
            var page = request.Page > totalPages ? totalPages : request.Page;
            
            // Obtener datos paginados con proyección
            var items = await productoEntityQuery
                .Skip((page - 1) * request.Size)
                .Take(request.Size)
                .Select(producto => new ProductoDTO
                {
                    Id = producto.Id,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Descripcion = producto.Descripcion,
                    Calidad = new CalidadRelacion
                    {
                        Id = producto.Calidad!.Id,
                        Codigo = producto.Calidad.Codigo
                    },
                    TipoProduccion = new TipoProduccionRelacion
                    {
                        Id = producto.TipoProduccion!.Id,
                        Codigo = producto.TipoProduccion.Codigo
                    },
                    Activo = producto.Activo,
                    CreadoPorId = producto.CreadoPorId,
                    CreadoEl = producto.CreadoEl,
                    ModificadoPorId = producto.ModificadoPorId,
                    ModificadoEl = producto.ModificadoEl
                })
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResponse<ProductoDTO>
            {
                Items = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = request.Size,
                    TotalCount = totalCount,
                    HasPrevious = page > 1,
                    HasNext = page < totalPages,
                    PreviousPage = page > 1 ? page - 1 : null,
                    NextPage = page < totalPages ? page + 1 : null
                }
            };
            return pagedResult;
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    // Orden helper eliminado; lógica inline arriba
}
