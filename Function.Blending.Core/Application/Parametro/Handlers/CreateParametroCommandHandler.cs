using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Functions.Support.Execution;
using Function.Blending.Core.Shared.Extensions;
using System.Security.Claims;

using Function.Blending.Core.Application.Parametro.Commands;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class CreateParametroCommandHandler : IRequestHandler<CreateParametroCommand, ParametroDTO>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly IFunctionContextAccessor _functionContextAccessor;

    public CreateParametroCommandHandler(IParametroRepository parametroRepository, IFunctionContextAccessor functionContextAccessor)
    {
        _parametroRepository = parametroRepository;
        _functionContextAccessor = functionContextAccessor;
    }

    public async Task<ParametroDTO> Handle(CreateParametroCommand request, CancellationToken cancellationToken)
    {
        var creadoPorId = GetCurrentUserId();

        var parametro = new ParametroEntity
        {
            Id = Guid.NewGuid(),
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Activo = request.Activo ?? true,
            CreadoPorId = creadoPorId,
            CreadoEl = DateTime.UtcNow
        };

        await _parametroRepository.CreateAsync(parametro);

        return new ParametroDTO
        {
            Id = parametro.Id,
            Codigo = parametro.Codigo,
            Nombre = parametro.Nombre,
            Descripcion = parametro.Descripcion,
            Activo = parametro.Activo,
            CreadoPorId = parametro.CreadoPorId,
            CreadoEl = parametro.CreadoEl,
            ModificadoPorId = parametro.ModificadoPorId,
            ModificadoEl = parametro.ModificadoEl
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
