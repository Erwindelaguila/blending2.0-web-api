using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class GetAllCalidadesWithoutPaginationQueryHandler : IRequestHandler<GetAllCalidadesWithoutPaginationQuery, List<CalidadDTO>>
{
    private readonly ICalidadRepository _calidadRepository;

    public GetAllCalidadesWithoutPaginationQueryHandler(ICalidadRepository calidadRepository)
    {
        _calidadRepository = calidadRepository;
    }

    public async Task<List<CalidadDTO>> Handle(GetAllCalidadesWithoutPaginationQuery request, CancellationToken cancellationToken)
    {
        var entities = await _calidadRepository.GetAllAsync();
        
        return entities.Select(calidad => new CalidadDTO
        {
            Id = calidad.Id,
            Codigo = calidad.Codigo,
            Nombre = calidad.Nombre,
            CodigoMaterial = calidad.CodigoMaterial,
            Descripcion = calidad.Descripcion,
            NoConforme = calidad.NoConforme,
            Activo = calidad.Activo,
            CreadoPorId = calidad.CreadoPorId,
            CreadoEl = calidad.CreadoEl,
            ModificadoPorId = calidad.ModificadoPorId,
            ModificadoEl = calidad.ModificadoEl
        }).ToList();
    }
}
