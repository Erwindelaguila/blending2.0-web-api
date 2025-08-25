using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class CreateProductoCommandHandler : IRequestHandler<CreateProductoCommand, ProductoDTO>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICalidadRepository _calidadRepository;
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    
    public CreateProductoCommandHandler(
        IProductoRepository productoRepository,
        ICalidadRepository calidadRepository,
        ITipoProduccionRepository tipoProduccionRepository)
    {
        _productoRepository = productoRepository;
        _calidadRepository = calidadRepository;
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<ProductoDTO> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        // Si se intenta crear activo, validar dependencias
        if (request.Activo ?? true)
        {
            await ValidateDependenciesForActivation(request.CalidadId, request.TipoProduccionId);
        }

        var producto = new ProductoEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            CalidadId = request.CalidadId,
            TipoProduccionId = request.TipoProduccionId,
            Activo = request.Activo ?? true,
            CreadoPorId = request.CreadoPorId,
            CreadoEl = DateTime.UtcNow
        };
        await _productoRepository.CreateAsync(producto);
        
        // Devolver la estructura anidada
        return await _productoRepository.GetByIdWithRelationsAsync(producto.Id) ?? 
               throw new InvalidOperationException("Error al crear el producto");
    }

    private async Task ValidateDependenciesForActivation(Guid calidadId, Guid tipoProduccionId)
    {
        // Validar Calidad
        var calidad = await _calidadRepository.GetByIdAsync(calidadId);
        if (calidad == null)
            throw new BusinessRuleException($"Calidad with ID {calidadId} not found.", 
                "CALIDAD_NOT_FOUND");
        
        if (calidad.Activo == false)
            throw new BusinessRuleException($"Cannot create/activate Producto because Calidad '{calidad.Nombre}' is inactive.", 
                "CALIDAD_INACTIVE");

        // Validar TipoProduccion
        var tipoProduccion = await _tipoProduccionRepository.GetByIdAsync(tipoProduccionId);
        if (tipoProduccion == null)
            throw new BusinessRuleException($"TipoProduccion with ID {tipoProduccionId} not found.", 
                "TIPO_PRODUCCION_NOT_FOUND");
        
        if (tipoProduccion.Activo == false)
            throw new BusinessRuleException($"Cannot create/activate Producto because TipoProduccion '{tipoProduccion.Nombre}' is inactive.", 
                "TIPO_PRODUCCION_INACTIVE");
    }
}
