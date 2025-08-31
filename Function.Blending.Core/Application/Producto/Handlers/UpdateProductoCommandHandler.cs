using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Producto.Commands;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using MediatR;

namespace Function.Blending.Core.Application.Producto.Handlers;

/// <summary>
/// Handler para actualizar productos
/// Incluye auditoría automática y validaciones de negocio
/// </summary>
public class UpdateProductoCommandHandler : IRequestHandler<UpdateProductoCommand, ProductoDTO>
{
    private readonly IProductoRepository _productoRepository;
    private readonly ICalidadRepository _calidadRepository;
    private readonly ITipoProduccionRepository _tipoProduccionRepository;
    private readonly IAuthorizationService _authorizationService;
    
    public UpdateProductoCommandHandler(
        IProductoRepository productoRepository,
        ICalidadRepository calidadRepository,
        ITipoProduccionRepository tipoProduccionRepository,
        IAuthorizationService authorizationService)
    {
        _productoRepository = productoRepository ?? throw new ArgumentNullException(nameof(productoRepository));
        _calidadRepository = calidadRepository ?? throw new ArgumentNullException(nameof(calidadRepository));
        _tipoProduccionRepository = tipoProduccionRepository ?? throw new ArgumentNullException(nameof(tipoProduccionRepository));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public async Task<ProductoDTO> Handle(UpdateProductoCommand request, CancellationToken cancellationToken)
    {
        // Obtener usuario actual para auditoría
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("ID de usuario inválido en headers");
        }

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
}
