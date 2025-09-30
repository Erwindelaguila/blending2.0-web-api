using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.AppParam.Commands;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Application.Common.Exceptions;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Constants;
using Function.Blending.Core.Shared.Extensions;
using MediatR;
using System.Security.Claims;

namespace Function.Blending.Core.Application.AppParam.Handlers;


public class UpdateAppParamCommandHandler : IRequestHandler<UpdateAppParamCommand, object>
{
    private readonly IAppParamRepository _appParamRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public UpdateAppParamCommandHandler(IAppParamRepository appParamRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _appParamRepository = appParamRepository ?? throw new ArgumentNullException(nameof(appParamRepository));
        _functionContextAccessor = functionContextAccessor ?? throw new ArgumentNullException(nameof(functionContextAccessor));
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

    public async Task<object> Handle(UpdateAppParamCommand request, CancellationToken cancellationToken)
    {
        
        var existingAppParam = await _appParamRepository.GetByKeyAsync(request.Key);
        
        if (existingAppParam == null)
        {
            throw new KeyNotFoundException($"AppParam with key '{request.Key}' not found");
        }

        // Determinar el código final (actual o nuevo)
        string finalKey = string.IsNullOrWhiteSpace(request.NewKey) ? request.Key : request.NewKey;
        bool isChangingKey = !string.IsNullOrWhiteSpace(request.NewKey) && request.NewKey != request.Key;

        // VALIDACIÓN DE NEGOCIO: Si isInternal = true → NO se puede modificar el código
        if (existingAppParam.IsInternal && isChangingKey)
        {
            throw new BusinessRuleException("No se puede modificar el código de parámetros internos", "INTERNAL_PARAM_KEY_IMMUTABLE");
        }

        // Si se está cambiando el código, validar que el nuevo no exista
        if (isChangingKey)
        {
            if (await _appParamRepository.ExistsAsync(finalKey))
            {
                throw new DuplicateKeyException("parámetro", finalKey);
            }
        }

        // ACTUALIZAR O RECREAR SEGÚN CORRESPONDA
        if (isChangingKey)
        {
            // Si cambia el código: ELIMINAR el viejo y CREAR uno nuevo
            await _appParamRepository.DeleteAsync(request.Key);
            
            var newAppParam = new AppParamEntity
            {
                Key = finalKey,
                Value = request.Value,
                Description = request.Description ?? existingAppParam.Description,
                Category = request.Category ?? existingAppParam.Category,
                Group = request.Group ?? existingAppParam.Group,
                IsActive = request.IsActive ?? existingAppParam.IsActive,
                IsInternal = existingAppParam.IsInternal,
                IsVisible = existingAppParam.IsVisible,
                IsDisableable = existingAppParam.IsDisableable,
                IsRemovable = existingAppParam.IsRemovable,
                CreadoPorId = existingAppParam.CreadoPorId,
                CreadoEl = existingAppParam.CreadoEl,
                ModificadoPorId = GetCurrentUserId(),
                ModificadoEl = DateTime.UtcNow
            };
            
            var createdAppParam = await _appParamRepository.CreateAndReturnAsync(newAppParam);
            
            return new AppParamDTO
            {
                Key = createdAppParam.Key,
                Value = createdAppParam.Value,
                Description = createdAppParam.Description,
                Category = createdAppParam.Category,
                Group = createdAppParam.Group,
                IsActive = createdAppParam.IsActive,
                IsInternal = createdAppParam.IsInternal,
                IsVisible = createdAppParam.IsVisible,
                IsDisableable = createdAppParam.IsDisableable,
                IsRemovable = createdAppParam.IsRemovable,
                CreadoPorId = createdAppParam.CreadoPorId,
                CreadoEl = createdAppParam.CreadoEl,
                ModificadoPorId = createdAppParam.ModificadoPorId,
                ModificadoEl = createdAppParam.ModificadoEl
            };
        }
        else
        {

            if (request.IsActive.HasValue && !request.IsActive.Value)
            {
                if (existingAppParam.IsDisableable)
                {
                    throw new BusinessRuleException("Este parámetro no puede ser desactivado (parámetro protegido)", "PROTECTED_PARAM_CANNOT_DISABLE");
                }
            }


            existingAppParam.Value = request.Value;
            if (request.Description != null)
                existingAppParam.Description = request.Description;
            if (request.Category != null)
                existingAppParam.Category = request.Category;
            if (request.Group != null)
                existingAppParam.Group = request.Group;
            if (request.IsActive.HasValue)
                existingAppParam.IsActive = request.IsActive.Value;
                
            existingAppParam.ModificadoPorId = GetCurrentUserId();
            existingAppParam.ModificadoEl = DateTime.UtcNow;

            var updatedAppParam = await _appParamRepository.UpdateAndReturnAsync(existingAppParam);

            return new AppParamDTO
            {
                Key = updatedAppParam.Key,
                Value = updatedAppParam.Value,
                Description = updatedAppParam.Description,
                Category = updatedAppParam.Category,
                Group = updatedAppParam.Group,
                IsActive = updatedAppParam.IsActive,
                IsInternal = updatedAppParam.IsInternal,
                IsVisible = updatedAppParam.IsVisible,
                IsDisableable = updatedAppParam.IsDisableable,
                IsRemovable = updatedAppParam.IsRemovable,
                CreadoPorId = updatedAppParam.CreadoPorId,
                CreadoEl = updatedAppParam.CreadoEl,
                ModificadoPorId = updatedAppParam.ModificadoPorId,
                ModificadoEl = updatedAppParam.ModificadoEl
            };
        }
    }
}