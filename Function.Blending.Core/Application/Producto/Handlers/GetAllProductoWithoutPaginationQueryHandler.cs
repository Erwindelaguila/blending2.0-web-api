using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Producto.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class GetAllProductoWithoutPaginationQueryHandler : IRequestHandler<GetAllProductoWithoutPaginationQuery, List<ProductoDTO>>
{
    private readonly IProductoRepository _productoRepository;

    public GetAllProductoWithoutPaginationQueryHandler(IProductoRepository productoRepository)
    {
        _productoRepository = productoRepository;
    }

    public async Task<List<ProductoDTO>> Handle(GetAllProductoWithoutPaginationQuery request, CancellationToken cancellationToken)
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

            // Filtro por CalidadId
            if (request.Filters.CalidadId.HasValue)
            {
                productoQuery = productoQuery.Where(x => x.CalidadId == request.Filters.CalidadId.Value);
            }

            // Filtro por TipoProduccionId  
            if (request.Filters.TipoProduccionId.HasValue)
            {
                productoQuery = productoQuery.Where(x => x.TipoProduccionId == request.Filters.TipoProduccionId.Value);
            }
        }

        // Ordenar por fecha de creación
        productoQuery = productoQuery.OrderBy(x => x.CreadoEl);

        var entities = await productoQuery.ToListAsync(cancellationToken);

        return entities.Select(producto => new ProductoDTO
        {
            Id = producto.Id,
            Codigo = producto.Codigo,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Activo = producto.Activo,
            CreadoPorId = producto.CreadoPorId,
            CreadoEl = producto.CreadoEl,
            ModificadoPorId = producto.ModificadoPorId,
            ModificadoEl = producto.ModificadoEl
        }).ToList();
    }
}
