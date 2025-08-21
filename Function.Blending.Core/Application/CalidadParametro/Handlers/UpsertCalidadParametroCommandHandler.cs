using Function.Blending.Core.Application.CalidadParametro.Commands;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Handlers;

public class UpsertCalidadParametroCommandHandler : IRequestHandler<UpsertCalidadParametroCommand, bool>
{
    private readonly ICalidadParametroRepository _calidadParametroRepository;
    private readonly ICalidadRepository _calidadRepository;
    private readonly IParametroRepository _parametroRepository;

    public UpsertCalidadParametroCommandHandler(
        ICalidadParametroRepository calidadParametroRepository,
        ICalidadRepository calidadRepository,
        IParametroRepository parametroRepository)
    {
        _calidadParametroRepository = calidadParametroRepository;
        _calidadRepository = calidadRepository;
        _parametroRepository = parametroRepository;
    }

    public async Task<bool> Handle(UpsertCalidadParametroCommand request, CancellationToken cancellationToken)
    {
        // Validar que la calidad existe y está activa
        var calidad = await _calidadRepository.GetByIdAsync(request.CalidadId);
        if (calidad == null || !calidad.Activo)
        {
            throw new ArgumentException("La calidad especificada no existe o no está activa.", nameof(request.CalidadId));
        }

        // Validar que el parámetro existe y está activo
        var parametro = await _parametroRepository.GetByIdAsync(request.ParametroId);
        if (parametro == null || !parametro.Activo)
        {
            throw new ArgumentException("El parámetro especificado no existe o no está activo.", nameof(request.ParametroId));
        }

        // Realizar el upsert
        await _calidadParametroRepository.UpsertAsync(
            request.CalidadId,
            request.ParametroId,
            request.Valor,
            request.ModificadoPorId);

        return true;
    }
}
