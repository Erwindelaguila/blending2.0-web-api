using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class GetParametroByIdQueryHandler : IRequestHandler<GetParametroByIdQuery, ParametroDTO?>
{
    private readonly IParametroRepository _parametroRepository;

    public GetParametroByIdQueryHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<ParametroDTO?> Handle(GetParametroByIdQuery request, CancellationToken cancellationToken)
    {
        var parametro = await _parametroRepository.GetByIdAsync(request.Id);
        
        if (parametro == null)
            return null;
            
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
