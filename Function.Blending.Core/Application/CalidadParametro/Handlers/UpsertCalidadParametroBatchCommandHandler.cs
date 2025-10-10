using Function.Blending.Core.Application.CalidadParametro.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Handlers;

public class UpsertCalidadParametroBatchCommandHandler : IRequestHandler<UpsertCalidadParametroBatchCommand, int>
{
    private readonly ICalidadParametroRepository _calidadParametroRepository;
    private readonly ICalidadRepository _calidadRepository;
    private readonly IParametroRepository _parametroRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public UpsertCalidadParametroBatchCommandHandler(
        ICalidadParametroRepository calidadParametroRepository,
        ICalidadRepository calidadRepository,
        IParametroRepository parametroRepository,
        IFunctionContextAccessor functionContextAccessor)
    {
        _calidadParametroRepository = calidadParametroRepository;
        _calidadRepository = calidadRepository;
        _parametroRepository = parametroRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<int> Handle(UpsertCalidadParametroBatchCommand request, CancellationToken cancellationToken)
    {
        if (request.Cambios == null || !request.Cambios.Any())
        {
            return 0; // No hay cambios que procesar
        }

        // Validar que todas las calidades existen y están activas
        var calidadIds = request.Cambios.Select(c => c.CalidadId).Distinct().ToList();
        var calidades = await _calidadRepository.GetAllAsync();
        var calidadesActivas = calidades.Where(c => c.Activo && calidadIds.Contains(c.Id)).ToList();
        
        if (calidadesActivas.Count != calidadIds.Count)
        {
            throw new ArgumentException("Una o más calidades especificadas no existen o no están activas.");
        }

        // Validar que todos los parámetros existen y están activos
        var parametroIds = request.Cambios.Select(c => c.ParametroId).Distinct().ToList();
        var parametros = await _parametroRepository.GetAllAsync();
        var parametrosActivos = parametros.Where(p => p.Activo && parametroIds.Contains(p.Id)).ToList();
        
        if (parametrosActivos.Count != parametroIds.Count)
        {
            throw new ArgumentException("Uno o más parámetros especificados no existen o no están activos.");
        }

        // Obtener el ID del usuario actual para la auditoría
        var currentUserId = GetCurrentUserId();

        // Convertir a tuplas para el repositorio
        var cambiosTuplas = request.Cambios.Select(c => (c.CalidadId, c.ParametroId, c.Valor)).ToList();

        // Realizar el batch upsert
        var processedCount = await _calidadParametroRepository.UpsertBatchAsync(cambiosTuplas, currentUserId);

        return processedCount;
    }

    private Guid GetCurrentUserId()
    {
        try
        {
            var context = _functionContextAccessor.Current;
            if (context?.Items.TryGetValue(Function.Blending.Core.Shared.Constants.MiscellaneousConstants.Principal, out var principalObj) == true &&
                principalObj is System.Security.Claims.ClaimsPrincipal principal)
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
