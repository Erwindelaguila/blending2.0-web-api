using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class GetAllCalidadesQueryHandler : IRequestHandler<GetAllCalidadesQuery, List<CalidadDTO>>
{
    private readonly ICalidadRepository _repository;

    public GetAllCalidadesQueryHandler(ICalidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CalidadDTO>> Handle(GetAllCalidadesQuery request, CancellationToken cancellationToken)
    {
        var calidades = await _repository.GetAllAsync();
        return calidades.Select(calidad => new CalidadDTO
        {
            Id = calidad.Id,
            Codigo = calidad.Codigo,
            Nombre = calidad.Nombre,
            Descripcion = calidad.Descripcion,
            Activo = calidad.Activo,
            CodigoMaterial = calidad.CodigoMaterial,
            ModificadoEl = calidad.ModificadoEl,
            CreadoEl = calidad.CreadoEl,
            ModificadoPorId = calidad.ModificadoPorId,
            NoConforme = calidad.NoConforme,
            CreadoPorId = calidad.CreadoPorId,
        }).ToList();
    }
}