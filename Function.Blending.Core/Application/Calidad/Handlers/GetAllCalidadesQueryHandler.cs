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

                calidadesQuery = calidadesQuery.ApplyFechaRangeFilterConTipo(
                    request.Filters.FechaDesde,
                    request.Filters.FechaHasta,
                    request.Filters.TipoFecha,
                    x => x.CreadoEl,
                    x => x.ModificadoEl);
            }

            // Aplicar ordenamiento optimizado
            calidadesQuery = ApplyOptimizedSorting(calidadesQuery, request.Filters?.Estado);

            // Proyectar a DTO (hacer antes de paginación para optimizar)
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
            // Re-lanzar ArgumentException para que sea manejada por la función HTTP como 400
            throw;
        }
    }

    private static IQueryable<CalidadEntity> ApplyOptimizedSorting(IQueryable<CalidadEntity> query, string? estado)
    {
        return estado switch
        {
            "1" => query.OrderByDescending(x => x.Activo).ThenBy(x => x.CreadoEl), // Activos primero
            "0" => query.OrderBy(x => x.Activo).ThenBy(x => x.CreadoEl),           // Inactivos primero
            _ => query.OrderBy(x => x.CreadoEl)                                    // Por defecto: por fecha
        };
    }
}