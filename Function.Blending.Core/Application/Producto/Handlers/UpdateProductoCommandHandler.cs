using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;

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
    private readonly IFunctionContextAccessor _functionContextAccessor;
    
    public UpdateProductoCommandHandler(
        IProductoRepository productoRepository,
        ICalidadRepository calidadRepository,
        ITipoProduccionRepository tipoProduccionRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        _calidadRepository = calidadRepository ?? throw new ArgumentNullException(nameof(calidadRepository));
        _tipoProduccionRepository = tipoProduccionRepository ?? throw new ArgumentNullException(nameof(tipoProduccionRepository));
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<ProductoDTO> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        // Obtener el Producto actual para comparar cambios
        var currentProducto = await _productoRepository.GetByIdAsync(request.Id);
        if (currentProducto == null)
            throw new BusinessRuleException($"El producto con ID {request.Id} no fue encontrado.", 
                "PRODUCTO_NOT_FOUND");

        // Si se intenta activar el Producto
        if (request.Activo == true && currentProducto.Activo == false)
        {
            await ValidateDependenciesForActivation(request.CalidadId, request.TipoProduccionId);
        }

        // Si se intenta cambiar Calidad o TipoProduccion cuando está activo
        if (currentProducto.Activo == true && 
            (request.CalidadId != currentProducto.CalidadId || 
             request.TipoProduccionId != currentProducto.TipoProduccionId))
        {
            await ValidateDependenciesForActivation(request.CalidadId, request.TipoProduccionId);
        }

        // Actualizar propiedades
        currentProducto.Codigo = request.Codigo;
        currentProducto.Nombre = request.Nombre;
        currentProducto.Descripcion = request.Descripcion;
        currentProducto.CalidadId = request.CalidadId;
        currentProducto.TipoProduccionId = request.TipoProduccionId;
        currentProducto.Activo = request.Activo ?? currentProducto.Activo;
        currentProducto.ModificadoPorId = currentUserId;
        currentProducto.ModificadoEl = DateTime.UtcNow;

        await _productoRepository.UpdateAsync(currentProducto);

        return await _productoRepository.GetByIdWithRelationsAsync(request.Id) ?? 
               throw new InvalidOperationException("Error al actualizar el producto");
    }

    private async Task ValidateDependenciesForActivation(Guid calidadId, Guid tipoProduccionId)
    {
        // Validar Calidad
        var calidad = await _calidadRepository.GetByIdAsync(calidadId);
        if (calidad == null)
            throw new BusinessRuleException($"La calidad seleccionada ya no existe o fue eliminada.", 
                "CALIDAD_NOT_FOUND");
        
        if (calidad.Activo == false)
            throw new BusinessRuleException($"No se puede activar el Producto porque la Calidad '{calidad.Nombre}' está inactiva.", 
                "CALIDAD_INACTIVE");

        // Validar TipoProduccion
        var tipoProduccion = await _tipoProduccionRepository.GetByIdAsync(tipoProduccionId);
        if (tipoProduccion == null)
            throw new BusinessRuleException($"El tipo de producción seleccionado ya no existe o fue eliminado.", 
                "TIPO_PRODUCCION_NOT_FOUND");
        
        if (tipoProduccion.Activo == false)
            throw new BusinessRuleException($"No se puede activar el Producto porque el Tipo de Producción '{tipoProduccion.Nombre}' está inactivo.", 
                "TIPO_PRODUCCION_INACTIVE");
    }

    private Guid GetCurrentUserId()
    {
        try
        {
            var context = _functionContextAccessor.Current;
            if (context?.Items.TryGetValue(MiscellaneousConstants.Principal, out var principalObj) == true &&
                principalObj is ClaimsPrincipal principal)
            {
                var userIdString = principal.GetUserId();
                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                {
                    return userId;
                }
            }
        }
        catch
        {
            // Si hay error obteniendo el usuario, usar fallback
        }
        
        // Fallback: usuario del sistema
        return Guid.Parse("00000000-0000-0000-0000-000000000001");
    }
}
