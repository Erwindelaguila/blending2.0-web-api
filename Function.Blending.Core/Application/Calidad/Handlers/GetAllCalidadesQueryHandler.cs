using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Calidad.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Common.Wrappers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using MediatR;

namespace Function.Blending.Core.Application.Calidad.Handlers;

public class GetAllCalidadesQueryHandler : IRequestHandler<GetAllCalidadesQuery, PagedResponse<CalidadDTO>>
{
    private readonly ICalidadRepository _calidadRepository;

    public GetAllCalidadesQueryHandler(ICalidadRepository calidadRepository)
    {
        _calidadRepository = calidadRepository;
    }

    public async Task<PagedResponse<CalidadDTO>> Handle(GetAllCalidadesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var calidadesQuery = _calidadRepository.GetQueryable();

            if (request.Filters != null)
            {
                calidadesQuery = calidadesQuery.ApplyCodigoFilter(
                    request.Filters.Codigo,
                    x => x.Codigo);

                calidadesQuery = calidadesQuery.ApplyEstadoFilter(
                    request.Filters.Estado,
                    x => x.Activo);

                if (request.Filters.FechaDesde.HasValue)
                {
                    var start = request.Filters.FechaDesde.Value.Date;
                    var end = start.AddDays(1);
                    calidadesQuery = calidadesQuery.Where(x =>
                        (x.CreadoEl >= start && x.CreadoEl < end) ||
                        (x.ModificadoEl.HasValue && x.ModificadoEl.Value >= start && x.ModificadoEl.Value < end)
                    );
                }
            }

            calidadesQuery = request.Filters?.Estado switch
            {
                "1" => calidadesQuery.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl),
                "0" => calidadesQuery.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),
                _ => calidadesQuery.OrderBy(x => x.CreadoEl)
            };

            var calidadesProjected = calidadesQuery.Select(calidad => new CalidadDTO
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
            });

            var pagedResult = await calidadesProjected.ToPagedResultAsync(request.Page, request.Size, cancellationToken);
            return pagedResult.ToPagedResponse();
        }
        catch (ArgumentException)
        {
            throw;
        }
    }

}