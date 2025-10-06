using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;

using Function.Blending.Core.Application.Parametro.Commands;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class UpdateParametroCommandHandler : IRequestHandler<UpdateParametroCommand, ParametroDTO>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public UpdateParametroCommandHandler(IParametroRepository parametroRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _parametroRepository = parametroRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<ParametroDTO> Handle(UpdateParametroCommand request, CancellationToken cancellationToken)
    {
        var modificadoPorId = GetCurrentUserId();

        var parametroToUpdate = new ParametroEntity
        {
            Id = request.Id,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo ?? true,
            ModificadoPorId = modificadoPorId,
            ModificadoEl = DateTime.UtcNow
        };

        var updatedParametro = await _parametroRepository.UpdateAndReturnAsync(parametroToUpdate);
        
        return new ParametroDTO
        {
            Id = updatedParametro.Id,
            Codigo = updatedParametro.Codigo,
            Nombre = updatedParametro.Nombre,
            Descripcion = updatedParametro.Descripcion,
            Activo = updatedParametro.Activo,
            CreadoPorId = updatedParametro.CreadoPorId,
            CreadoEl = updatedParametro.CreadoEl,
            ModificadoPorId = updatedParametro.ModificadoPorId,
            ModificadoEl = updatedParametro.ModificadoEl
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
