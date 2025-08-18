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

            productoQuery = productoQuery.ApplyFechaRangeFilterConTipo(
                request.Filters.FechaDesde,
                request.Filters.FechaHasta,
                request.Filters.TipoFecha,
                x => x.CreadoEl,
                x => x.ModificadoEl);
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
