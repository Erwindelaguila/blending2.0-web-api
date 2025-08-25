using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, ProductoDTO>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICalidadRepository _calidadRepository;
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    
    public UpdateProductoCommandHandler(
        IProductoRepository productoRepository,
        ICalidadRepository calidadRepository,
        ITipoProduccionRepository tipoProduccionRepository)
    {
        _productoRepository = productoRepository;
        _calidadRepository = calidadRepository;
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<ProductoDTO> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
    {
        var producto = await _productoRepository.GetByIdAsync(request.Id);
        if (producto == null)
            throw new ArgumentException($"Producto con ID {request.Id} no encontrado");

        // Obtener nuevos valores o mantener los actuales
        var newCalidadId = request.CalidadId ?? producto.CalidadId;
        var newTipoProduccionId = request.TipoProduccionId ?? producto.TipoProduccionId;
        var newActivo = request.Activo ?? producto.Activo;

        // Si se intenta activar el Producto
        if (newActivo == true && producto.Activo == false)
        {
            await ValidateDependenciesForActivation(newCalidadId, newTipoProduccionId);
        }

        // Si se cambian las dependencias mientras está activo
        if (producto.Activo == true && 
            (newCalidadId != producto.CalidadId || 
             newTipoProduccionId != producto.TipoProduccionId))
        {
            await ValidateDependenciesForActivation(newCalidadId, newTipoProduccionId);
        }

        producto.Codigo = request.Codigo ?? producto.Codigo;
        producto.Nombre = request.Nombre ?? producto.Nombre;
        producto.Descripcion = request.Descripcion ?? producto.Descripcion;
        producto.CalidadId = newCalidadId;
        producto.TipoProduccionId = newTipoProduccionId;
        producto.Activo = newActivo;
        producto.ModificadoPorId = request.ModificadoPorId;
        producto.ModificadoEl = DateTime.UtcNow;

        await _productoRepository.UpdateAsync(producto);

        // Devolver la estructura anidada
        return await _productoRepository.GetByIdWithRelationsAsync(producto.Id) ?? 
               throw new InvalidOperationException("Error al actualizar el producto");
    }

    private async Task ValidateDependenciesForActivation(Guid calidadId, Guid tipoProduccionId)
    {
        // Validar Calidad
        var calidad = await _calidadRepository.GetByIdAsync(calidadId);
        if (calidad == null)
            throw new BusinessRuleException($"Calidad with ID {calidadId} not found.", 
                "CALIDAD_NOT_FOUND");
        
        if (calidad.Activo == false)
            throw new BusinessRuleException($"Cannot activate Producto because Calidad '{calidad.Nombre}' is inactive.", 
                "CALIDAD_INACTIVE");

        // Validar TipoProduccion
        var tipoProduccion = await _tipoProduccionRepository.GetByIdAsync(tipoProduccionId);
        if (tipoProduccion == null)
            throw new BusinessRuleException($"TipoProduccion with ID {tipoProduccionId} not found.", 
                "TIPO_PRODUCCION_NOT_FOUND");
        
        if (tipoProduccion.Activo == false)
            throw new BusinessRuleException($"Cannot activate Producto because TipoProduccion '{tipoProduccion.Nombre}' is inactive.", 
                "TIPO_PRODUCCION_INACTIVE");
    }
}
