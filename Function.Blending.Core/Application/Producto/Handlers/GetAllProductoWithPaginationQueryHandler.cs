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

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
                    productoQuery = productoQuery.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl.Value >= start && x.ModificadoEl.Value < end)
                    );
                }
            }

            // Orden: por estado si viene, si no por CreadoEl
            productoQuery = request.Filters?.Estado switch
            {
                "1" => productoQuery.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl),
                "0" => productoQuery.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),
                _ => productoQuery.OrderBy(x => x.CreadoEl)
            };

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

    // Orden helper eliminado; lógica inline arriba
}
