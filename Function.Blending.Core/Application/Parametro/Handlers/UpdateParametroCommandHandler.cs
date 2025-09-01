using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Parametro.Commands;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class UpdateParametroCommandHandler : IRequestHandler<UpdateParametroCommand, ParametroDTO>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly IAuthorizationService _authorizationService;

    public UpdateParametroCommandHandler(IParametroRepository parametroRepository, IAuthorizationService authorizationService)
    {
        _parametroRepository = parametroRepository;
        _authorizationService = authorizationService;
    }

    public async Task<ParametroDTO> Handle(UpdateParametroCommand request, CancellationToken cancellationToken)
    {
        var modificadoPorIdString = _authorizationService.GetCurrentUserId();
        var modificadoPorId = Guid.Parse(modificadoPorIdString);

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
}
