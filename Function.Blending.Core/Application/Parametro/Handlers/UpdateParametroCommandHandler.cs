using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Parametro.Commands;
using Function.Blending.Core.Application.Parametro.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class UpdateParametroCommandHandler : IRequestHandler<UpdateParametroCommand, ParametroDTO>
{
    private readonly IParametroRepository _parametroRepository;

    public UpdateParametroCommandHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<ParametroDTO> Handle(UpdateParametroCommand request, CancellationToken cancellationToken)
    {
        var parametro = await _parametroRepository.GetByIdAsync(request.Id);
        
        if (parametro == null)
            throw new ArgumentException($"Parametro con ID {request.Id} no encontrado");

        parametro.Codigo = request.Codigo ?? parametro.Codigo;
        parametro.Nombre = request.Nombre ?? parametro.Nombre;
        parametro.Descripcion = request.Descripcion ?? parametro.Descripcion;
        parametro.Activo = request.Activo ?? parametro.Activo;
        parametro.ModificadoPorId = request.ModificadoPorId;
        parametro.ModificadoEl = DateTime.Now;

        await _parametroRepository.UpdateAsync(parametro);
        
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
