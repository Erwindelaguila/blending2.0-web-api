using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Application.Planta.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Planta.Handlers;

public class GetPagedPlantasQueryHandler : IRequestHandler<GetPagedPlantasQuery, object>
{
    private readonly IPlantaRepository _plantaRepository;
    private readonly IMapper _mapper;

    public GetPagedPlantasQueryHandler(IPlantaRepository plantaRepository, IMapper mapper)
    {
        _plantaRepository = plantaRepository;
        _mapper = mapper;
    }

    public async Task<object> Handle(GetPagedPlantasQuery request, CancellationToken cancellationToken)
    {
        var (entities, total) = await _plantaRepository.GetPagedAsync(request.Page, request.Size);
        
        var dtos = entities.Select(planta => new PlantaDTO
        {
            Id = planta.Id,
            Codigo = planta.Codigo,
            Nombre = planta.Nombre,
            Descripcion = planta.Descripcion,
            NumeroRuma = planta.NumeroRuma,
            Activo = planta.Activo,
            CreadoPorId = planta.CreadoPorId,
            CreadoEl = planta.CreadoEl,
            ModificadoPorId = planta.ModificadoPorId,
            ModificadoEl = planta.ModificadoEl
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
