using Function.Blending.Core.Application.CalidadParametro.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Handlers;

public class UpsertCalidadParametroCommandHandler : IRequestHandler<UpsertCalidadParametroCommand, bool>
{
    private readonly ICalidadParametroRepository _calidadParametroRepository;
    private readonly ICalidadRepository _calidadRepository;
    private readonly IParametroRepository _parametroRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public UpsertCalidadParametroCommandHandler(
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

    public async Task<bool> Handle(UpsertCalidadParametroCommand request, CancellationToken cancellationToken)
    {
        var calidad = await _calidadRepository.GetByIdAsync(request.CalidadId);
        if (calidad == null || !calidad.Activo)
        {
            throw new ArgumentException("La calidad especificada no existe o no está activa.", nameof(request.CalidadId));
        }

        var parametro = await _parametroRepository.GetByIdAsync(request.ParametroId);
        if (parametro == null || !parametro.Activo)
        {
            throw new ArgumentException("El parámetro especificado no existe o no está activo.", nameof(request.ParametroId));
        }

        var currentUserId = GetCurrentUserId();

        await _calidadParametroRepository.UpsertAsync(
            request.CalidadId,
            request.ParametroId,
            request.Valor,
            currentUserId);

        return true;
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
