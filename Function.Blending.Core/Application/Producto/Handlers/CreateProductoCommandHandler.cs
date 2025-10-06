using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;

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
    private readonly IFunctionContextAccessor _functionContextAccessor;
    
    public CreateProductoCommandHandler(
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

    public async Task<ProductoDTO> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = GetCurrentUserId();

        if (await _productoRepository.ExistsActiveCodigoAsync(request.Codigo))
        {
            throw new ArgumentException("El código ya existe", "codigo");
        }

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
            CreadoPorId = currentUserId,
            CreadoEl = DateTime.UtcNow
        };
        
        await _productoRepository.CreateAsync(producto);
        
        return await _productoRepository.GetByIdWithRelationsAsync(producto.Id) ?? 
               throw new InvalidOperationException("Error al crear el producto");
    }

    private async Task ValidateDependenciesForActivation(Guid calidadId, Guid tipoProduccionId)
    {
        var calidad = await _calidadRepository.GetByIdAsync(calidadId);
        if (calidad == null)
            throw new BusinessRuleException($"La calidad seleccionada ya no existe o fue eliminada.", 
                "CALIDAD_NOT_FOUND");
        
        if (calidad.Activo == false)
            throw new BusinessRuleException($"No se puede crear/activar el Producto porque la Calidad '{calidad.Nombre}' está inactiva.", 
                "CALIDAD_INACTIVE");

        var tipoProduccion = await _tipoProduccionRepository.GetByIdAsync(tipoProduccionId);
        if (tipoProduccion == null)
            throw new BusinessRuleException($"El tipo de producción seleccionado ya no existe o fue eliminado.", 
                "TIPO_PRODUCCION_NOT_FOUND");
        
        if (tipoProduccion.Activo == false)
            throw new BusinessRuleException($"No se puede crear/activar el Producto porque el Tipo de Producción '{tipoProduccion.Nombre}' está inactivo.", 
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
