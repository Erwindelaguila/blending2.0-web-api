using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
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
    private readonly IAuthorizationService _authorizationService;
    
    public CreateProductoCommandHandler(
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

    public async Task<ProductoDTO> Handle(CreateProductoCommand request, CancellationToken cancellationToken)
    {
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("ID de usuario inválido en headers");
        }

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
}
