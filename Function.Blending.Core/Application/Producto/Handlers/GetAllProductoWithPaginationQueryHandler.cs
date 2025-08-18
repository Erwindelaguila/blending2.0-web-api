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
            var productoQuery = _productoRepository.GetQueryable();

            // Aplicar filtros si existen
            if (request.Filters != null)
            {
                productoQuery = productoQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                productoQuery = productoQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                productoQuery = productoQuery.ApplyFechaRangeFilterConTipo(
                    request.Filters.FechaDesde,
                    request.Filters.FechaHasta,
                    request.Filters.TipoFecha,
                    x => x.CreadoEl,
                    x => x.ModificadoEl);
            }

            // Aplicar ordenamiento optimizado
            productoQuery = ApplyOptimizedSorting(productoQuery, request.Filters?.Estado);

            // Proyectar a DTO antes de paginar
            var productoProjected = productoQuery.Select(producto => new ProductoDTO
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                CalidadId = producto.CalidadId,
                TipoProduccionId = producto.TipoProduccionId,
                Activo = producto.Activo,
                CreadoPorId = producto.CreadoPorId,
                CreadoEl = producto.CreadoEl,
                ModificadoPorId = producto.ModificadoPorId,
                ModificadoEl = producto.ModificadoEl
            });

            var pagedResult = await productoProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    private static IQueryable<Domain.Entities.ProductoEntity> ApplyOptimizedSorting(
        IQueryable<Domain.Entities.ProductoEntity> query, 
        string? estado)
    {
        return estado switch
        {
            "1" => query.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl), // Activos primero
            "0" => query.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),           // Inactivos primero
            _ => query.OrderBy(x => x.CreadoEl)                                    // Por defecto: por fecha
        };
    }
}
