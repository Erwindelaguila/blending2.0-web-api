using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Agregado.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Handlers;

public class GetAllAgregadosQueryHandler : IRequestHandler<GetAllAgregadosQuery, PagedResponse<AgregadoDTO>>
{
    private readonly IAgregadoRepository _agregadoRepository;

    public GetAllAgregadosQueryHandler(IAgregadoRepository agregadoRepository)
    {
        _agregadoRepository = agregadoRepository;
    }

    public async Task<PagedResponse<AgregadoDTO>> Handle(GetAllAgregadosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var agregadosQuery = _agregadoRepository.GetQueryable();

            if (request.Filters != null)
            {
                agregadosQuery = agregadosQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                agregadosQuery = agregadosQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                if (request.Filters.FechaDesde.HasValue)
                {
                    var fechaEspecifica = request.Filters.FechaDesde.Value.Date;
                    var fechaSiguiente = fechaEspecifica.AddDays(1);
                    agregadosQuery = agregadosQuery.Where(x => 
                        (x.CreadoEl >= fechaEspecifica && x.CreadoEl < fechaSiguiente) ||
                        (x.ModificadoEl != null && x.ModificadoEl.Value >= fechaEspecifica && x.ModificadoEl.Value < fechaSiguiente)
                    );
                }
            }

            agregadosQuery = agregadosQuery.OrderBy(x => x.CreadoEl);

            var agregadosProjected = agregadosQuery.Select(agregado => new AgregadoDTO
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
            });

            var pagedResult = await agregadosProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            throw;
        }
    }
}
