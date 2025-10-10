using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;


public class CreateAppParamCommandHandler : IRequestHandler<CreateAppParamCommand, AppParamDTO>
{
    private readonly IAppParamRepository _appParamRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public CreateAppParamCommandHandler(IAppParamRepository appParamRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _appParamRepository = appParamRepository ?? throw new ArgumentNullException(nameof(appParamRepository));
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<AppParamDTO> Handle(CreateAppParamCommand request, CancellationToken cancellationToken)
    {
     
        var exists = await _appParamRepository.ExistsAsync(request.Key);
        if (exists)
        {
            throw new DuplicateKeyException("parámetro", request.Key);
        }

     
        var appParam = new AppParamEntity
        {
            Key = request.Key,
            Value = request.Value,
            Description = request.Description,
            Category = request.Category,
            Group = request.Group,
            // USUARIO CREA TODO PERMISIVO
            IsActive = true,                    // Siempre activo
            IsInternal = false,                 // Siempre false = puede modificar código  
            IsVisible = true,                   // Siempre visible
            IsDisableable = false,              // Siempre false = SÍ se puede desactivar (editable)
            IsRemovable = true,                 // Siempre true = se puede eliminar
            // CAMPOS DE AUDITORÍA AUTOMÁTICOS
            CreadoPorId = GetCurrentUserId(),   // Usuario real del JWT
            CreadoEl = DateTime.UtcNow
        };

        await _appParamRepository.CreateAsync(appParam);

        return new AppParamDTO
        {
            Key = appParam.Key,
            Value = appParam.Value,
            Description = appParam.Description,
            Category = appParam.Category,
            Group = appParam.Group,
            IsActive = appParam.IsActive,
            IsInternal = appParam.IsInternal,
            IsVisible = appParam.IsVisible,
            IsDisableable = appParam.IsDisableable,
            IsRemovable = appParam.IsRemovable,
            CreadoPorId = appParam.CreadoPorId,
            CreadoEl = appParam.CreadoEl,
            ModificadoPorId = appParam.ModificadoPorId,
            ModificadoEl = appParam.ModificadoEl
        };
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
