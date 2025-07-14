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

        var calidadesDTO = calidades.Select(calidades => new CalidadDTO
        {
            Id = calidades.Id,
            Codigo = calidades.Codigo,
            Nombre = calidades.Nombre,
            Descripcion = calidades.Descripcion,
            Activo = calidades.Activo,
            CodigoMaterial = calidades.CodigoMaterial,
            ModificadoEl = calidades.ModificadoEl,
            CreadoEl = calidades.CreadoEl,
            ModificadoPorId = calidades.ModificadoPorId,
            NoConforme = calidades.NoConforme,
            CreadoPorId = calidades.CreadoPorId,
        }).ToList();
        
        return calidadesDTO;
    }
}