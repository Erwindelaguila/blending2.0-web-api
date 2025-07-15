using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class GetAllParametrosQueryHandler : IRequestHandler<GetAllParametrosQuery, List<ParametroDTO>>
{
    private readonly IParametroRepository _parametroRepository;

    public GetAllParametrosQueryHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<List<ParametroDTO>> Handle(GetAllParametrosQuery request, CancellationToken cancellationToken)
    {
        var parametros = await _parametroRepository.GetAllAsync();
        
        return parametros.Select(parametro => new ParametroDTO
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
        }).ToList();
    }
}
