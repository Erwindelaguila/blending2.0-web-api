using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.AppParam.Handlers;


public class CreateAppParamCommandHandler : IRequestHandler<CreateAppParamCommand, AppParamDTO>
{
    private readonly IAppParamRepository _appParamRepository;
    private readonly IAuthorizationService _authorizationService;

    public CreateAppParamCommandHandler(
        IAppParamRepository appParamRepository,
        IAuthorizationService authorizationService)
    {
        _appParamRepository = appParamRepository ?? throw new ArgumentNullException(nameof(appParamRepository));
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    public async Task<AppParamDTO> Handle(CreateAppParamCommand request, CancellationToken cancellationToken)
    {
     
        var exists = await _appParamRepository.ExistsAsync(request.Key);
        if (exists)
        {
            throw new DuplicateKeyException("parámetro", request.Key);
        }

     
        var currentUserIdString = _authorizationService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserIdString, out var currentUserId))
        {
            throw new UnauthorizedAccessException("User ID inválido en headers");
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
            CreadoPorId = currentUserId,        // Del token JWT
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
}
