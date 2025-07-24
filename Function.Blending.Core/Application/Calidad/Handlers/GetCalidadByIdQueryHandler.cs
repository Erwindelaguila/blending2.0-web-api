using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class GetCalidadByIdQueryHandler : IRequestHandler<GetCalidadByIdQuery, CalidadDTO?>
{
    private readonly ICalidadRepository _calidadRepository;

    public GetCalidadByIdQueryHandler(ICalidadRepository calidadRepository)
    {
        _calidadRepository = calidadRepository;
    }

    public async Task<CalidadDTO?> Handle(GetCalidadByIdQuery request, CancellationToken cancellationToken)
    {
        var calidad = await _calidadRepository.GetByIdAsync(request.Id);
        if (calidad == null)
            return null;
        return new CalidadDTO
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
        };
    }
}
