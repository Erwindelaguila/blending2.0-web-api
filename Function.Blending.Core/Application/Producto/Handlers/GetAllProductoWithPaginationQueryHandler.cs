using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Producto.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

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
            // Obtener todos los datos con estructura anidada
            var allProductos = await _productoRepository.GetAllWithRelationsAsync();
            
            // Aplicar filtros en memoria usando request.Filters
            if (request.Filters != null)
            {
                if (!string.IsNullOrWhiteSpace(request.Filters.Codigo))
                    allProductos = allProductos.Where(x => x.Codigo.Contains(request.Filters.Codigo, StringComparison.OrdinalIgnoreCase)).ToList();
                
                if (!string.IsNullOrWhiteSpace(request.Filters.Estado))
                {
                    var isActivo = request.Filters.Estado == "1" || request.Filters.Estado.ToLower() == "activo";
                    allProductos = allProductos.Where(x => x.Activo == isActivo).ToList();
                }

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
                    allProductos = allProductos.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl >= start && x.ModificadoEl < end)
                    ).ToList();
                }
            }

            // Aplicar paginación
            var totalRecords = allProductos.Count;
            var productosPaginated = allProductos
                .Skip((request.Page - 1) * request.Size)
                .Take(request.Size)
                .ToList();

            return new PagedResponse<ProductoDTO>
            {
                Items = productosPaginated,
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
            throw new Exception("Error al obtener los productos paginados", ex);
        }
    }
}
