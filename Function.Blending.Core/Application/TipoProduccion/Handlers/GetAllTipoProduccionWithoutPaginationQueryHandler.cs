using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Application.TipoProduccion.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Application.TipoProduccion.Handlers;

public class GetAllTipoProduccionWithoutPaginationQueryHandler : IRequestHandler<GetAllTipoProduccionWithoutPaginationQuery, List<TipoProduccionDTO>>
{
    private readonly ITipoProduccionRepository _tipoProduccionRepository;

    public GetAllTipoProduccionWithoutPaginationQueryHandler(ITipoProduccionRepository tipoProduccionRepository)
    {
        _tipoProduccionRepository = tipoProduccionRepository;
    }

    public async Task<List<TipoProduccionDTO>> Handle(GetAllTipoProduccionWithoutPaginationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var tipoProduccionQuery = _tipoProduccionRepository.GetQueryable();

            if (request.Filters != null)
            {
                tipoProduccionQuery = tipoProduccionQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                tipoProduccionQuery = tipoProduccionQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
                    tipoProduccionQuery = tipoProduccionQuery.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl.Value >= start && x.ModificadoEl.Value < end)
                    );
                }
            }

            // Orden: por estado si viene, si no por CreadoEl
            tipoProduccionQuery = request.Filters?.Estado switch
            {
                "1" => tipoProduccionQuery.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl),
                "0" => tipoProduccionQuery.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),
                _ => tipoProduccionQuery.OrderBy(x => x.CreadoEl)
            };

            // Proyectar a DTO y ejecutar
            var tipoProduccionList = await tipoProduccionQuery.Select(tipoProduccion => new TipoProduccionDTO
            {
                Id = tipoProduccion.Id,
                Codigo = tipoProduccion.Codigo,
                Nombre = tipoProduccion.Nombre,
                Descripcion = tipoProduccion.Descripcion,
                LineaProduccionId = tipoProduccion.LineaProduccionId,
                AgregadoId = tipoProduccion.AgregadoId,
                Activo = tipoProduccion.Activo,
                CreadoPorId = tipoProduccion.CreadoPorId,
                CreadoEl = tipoProduccion.CreadoEl,
                ModificadoPorId = tipoProduccion.ModificadoPorId,
                ModificadoEl = tipoProduccion.ModificadoEl
            }).ToListAsync(cancellationToken);

            return tipoProduccionList;
        }
        catch (ArgumentException)
        {
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }
}
