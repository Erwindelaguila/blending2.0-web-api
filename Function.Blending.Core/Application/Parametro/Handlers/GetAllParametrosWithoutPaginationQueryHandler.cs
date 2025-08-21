using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class GetAllParametrosWithoutPaginationQueryHandler : IRequestHandler<GetAllParametrosWithoutPaginationQuery, List<ParametroDTO>>
{
    private readonly IParametroRepository _parametroRepository;

    public GetAllParametrosWithoutPaginationQueryHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<List<ParametroDTO>> Handle(GetAllParametrosWithoutPaginationQuery request, CancellationToken cancellationToken)
    {
        var entities = await _parametroRepository.GetAllAsync();
        
        return entities.Select(parametro => new ParametroDTO
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

        return new
        {
            Items = dtos,
            Total = total,
            Page = request.Page,
            Size = request.Size,
            TotalPages = (int)Math.Ceiling((double)total / request.Size)
        };
    }
}
