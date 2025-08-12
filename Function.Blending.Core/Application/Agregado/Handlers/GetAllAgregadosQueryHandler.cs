using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class GetAllAgregadosQueryHandler : IRequestHandler<GetAllAgregadosQuery, object>
{
    private readonly IAgregadoRepository _agregadoRepository;

    public GetAllAgregadosQueryHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }

    public async Task<object> Handle(GetAllAgregadosQuery request, CancellationToken cancellationToken)
    {
        var (entities, total) = await _agregadoRepository.GetPagedAsync(request.Page, request.Size);
        
        var dtos = entities.Select(agregado => new AgregadoDTO
        {
            Id = agregado.Id,
            Codigo = agregado.Codigo,
            Nombre = agregado.Nombre,
            Descripcion = agregado.Descripcion,
            Activo = agregado.Activo,
            CreadoPorId = agregado.CreadoPorId,
            CreadoEl = agregado.CreadoEl,
            ModificadoPorId = agregado.ModificadoPorId,
            ModificadoEl = agregado.ModificadoEl
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
