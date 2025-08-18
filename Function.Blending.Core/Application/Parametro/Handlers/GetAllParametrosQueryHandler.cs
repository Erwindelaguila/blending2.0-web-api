using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Parametro.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Handlers;

public class GetAllParametrosQueryHandler : IRequestHandler<GetAllParametrosQuery, object>
{
    private readonly IParametroRepository _parametroRepository;

    public GetAllParametrosQueryHandler(IParametroRepository parametroRepository)
    {
        _parametroRepository = parametroRepository;
    }

    public async Task<object> Handle(GetAllParametrosQuery request, CancellationToken cancellationToken)
    {
        var (entities, total) = await _parametroRepository.GetPagedAsync(request.Page, request.Size);
        
        var dtos = entities.Select(parametro => new ParametroDTO
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
