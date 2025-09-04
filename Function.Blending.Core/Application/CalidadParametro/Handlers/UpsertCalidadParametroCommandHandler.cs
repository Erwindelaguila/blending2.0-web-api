using Function.Blending.Core.Application.CalidadParametro.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Interfaces.Services;
using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Handlers;

public class UpsertCalidadParametroCommandHandler : IRequestHandler<UpsertCalidadParametroCommand, bool>
{
    private readonly ICalidadParametroRepository _calidadParametroRepository;
    private readonly ICalidadRepository _calidadRepository;
    private readonly IParametroRepository _parametroRepository;
    private readonly IAuthorizationService _authorizationService;

    public UpsertCalidadParametroCommandHandler(
        ICalidadParametroRepository calidadParametroRepository,
        ICalidadRepository calidadRepository,
        IParametroRepository parametroRepository,
        IAuthorizationService authorizationService)
    {
        _calidadParametroRepository = calidadParametroRepository;
        _calidadRepository = calidadRepository;
        _parametroRepository = parametroRepository;
        _authorizationService = authorizationService;
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

        var currentUserIdString = _authorizationService.GetCurrentUserId();
        var currentUserId = Guid.Parse(currentUserIdString);

        await _calidadParametroRepository.UpsertAsync(
            request.CalidadId,
            request.ParametroId,
            request.Valor,
            currentUserId);

        return true;
    }
}
