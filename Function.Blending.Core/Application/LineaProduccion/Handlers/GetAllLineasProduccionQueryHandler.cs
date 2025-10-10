using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.LineaProduccion.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.LineaProduccion.Handlers;

public class GetAllLineasProduccionQueryHandler : IRequestHandler<GetAllLineasProduccionQuery, PagedResponse<LineaProduccionDTO>>
{
    private readonly ILineaProduccionRepository _lineaProduccionRepository;

    public GetAllLineasProduccionQueryHandler(ILineaProduccionRepository lineaProduccionRepository)
    {
        _lineaProduccionRepository = lineaProduccionRepository;
    }

    public async Task<PagedResponse<LineaProduccionDTO>> Handle(GetAllLineasProduccionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var lineasProduccionQuery = _lineaProduccionRepository.GetQueryable();

            if (request.Filters != null)
            {
                lineasProduccionQuery = lineasProduccionQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                lineasProduccionQuery = lineasProduccionQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
                    lineasProduccionQuery = lineasProduccionQuery.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl.Value >= start && x.ModificadoEl.Value < end)
                    );
                }
            }

            lineasProduccionQuery = request.Filters?.Estado switch
            {
                "1" => lineasProduccionQuery.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl),
                "0" => lineasProduccionQuery.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),
                _ => lineasProduccionQuery.OrderBy(x => x.CreadoEl)
            };

            var lineasProduccionProjected = lineasProduccionQuery.Select(lineaProduccion => new LineaProduccionDTO
            {
                Id = lineaProduccion.Id,
                Codigo = lineaProduccion.Codigo,
                Nombre = lineaProduccion.Nombre,
                Descripcion = lineaProduccion.Descripcion,
                Activo = lineaProduccion.Activo,
                CreadoPorId = lineaProduccion.CreadoPorId,
                CreadoEl = lineaProduccion.CreadoEl,
                ModificadoPorId = lineaProduccion.ModificadoPorId,
                ModificadoEl = lineaProduccion.ModificadoEl
            });

            cancellationToken.ThrowIfCancellationRequested();

            var pagedResult = await lineasProduccionProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (OperationCanceledException)
        {
            throw; 
        }
        catch (ArgumentException)
        {
            throw; 
        }
    }
}
