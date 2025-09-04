using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Parametro.Commands;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class CreateParametroCommandHandler : IRequestHandler<CreateParametroCommand, ParametroDTO>
{
    private readonly IParametroRepository _parametroRepository;
    private readonly IAuthorizationService _authorizationService;

    public CreateParametroCommandHandler(IParametroRepository parametroRepository, IAuthorizationService authorizationService)
    {
        _parametroRepository = parametroRepository;
        _authorizationService = authorizationService;
    }

    public async Task<ParametroDTO> Handle(CreateParametroCommand request, CancellationToken cancellationToken)
    {
        var creadoPorIdString = _authorizationService.GetCurrentUserId();
        var creadoPorId = Guid.Parse(creadoPorIdString);

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
}
