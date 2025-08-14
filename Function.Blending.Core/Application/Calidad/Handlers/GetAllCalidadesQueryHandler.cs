using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class GetAllCalidadesQueryHandler : IRequestHandler<GetAllCalidadesQuery, object>
{
    private readonly ICalidadRepository _repository;

    public GetAllCalidadesQueryHandler(ICalidadRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> Handle(GetAllCalidadesQuery request, CancellationToken cancellationToken)
    {
        var (entities, total) = await _repository.GetPagedAsync(request.Page, request.Size);

        var dtos = entities.Select(calidad => new CalidadDTO
        {
            Id = calidad.Id,
            Codigo = calidad.Codigo,
            Nombre = calidad.Nombre,
            Descripcion = calidad.Descripcion,
            Activo = calidad.Activo,
            CodigoMaterial = calidad.CodigoMaterial,
            NoConforme = calidad.NoConforme,
            CreadoPorId = calidad.CreadoPorId,
            CreadoEl = calidad.CreadoEl,
            ModificadoPorId = calidad.ModificadoPorId,
            ModificadoEl = calidad.ModificadoEl
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