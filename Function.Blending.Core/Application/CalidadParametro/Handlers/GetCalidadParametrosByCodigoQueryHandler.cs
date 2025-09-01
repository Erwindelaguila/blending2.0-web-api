using Function.Blending.Core.Application.CalidadParametro.DTOs;
using Function.Blending.Core.Application.CalidadParametro.Queries;
using Function.Blending.Core.Application.Common.Helpers;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Handlers;

public class GetCalidadParametrosByCodigoQueryHandler : IRequestHandler<GetCalidadParametrosByCodigoQuery, List<CalidadParametroItemDTO>>
{
    private readonly ICalidadRepository _calidadRepository;
    private readonly IParametroRepository _parametroRepository;
    private readonly ICalidadParametroRepository _calidadParametroRepository;

    public GetCalidadParametrosByCodigoQueryHandler(
        ICalidadRepository calidadRepository,
        IParametroRepository parametroRepository,
        ICalidadParametroRepository calidadParametroRepository)
    {
        _calidadRepository = calidadRepository;
        _parametroRepository = parametroRepository;
        _calidadParametroRepository = calidadParametroRepository;
    }

    public async Task<List<CalidadParametroItemDTO>> Handle(GetCalidadParametrosByCodigoQuery request, CancellationToken cancellationToken)
    {
        var calidadesQuery = _calidadRepository.GetQueryable()
            .ApplyCodigoFilter(request.CodigoCalidad, c => c.Codigo)
            .Where(c => c.Activo);

        var calidades = await Task.FromResult(calidadesQuery.ToList());
        
        if (!calidades.Any())
        {
            return new List<CalidadParametroItemDTO>();
        }

        // Obtener todos los parámetros activos (catálogo maestro)
        var parametros = await _parametroRepository.GetAllAsync();
        var parametrosActivos = parametros.Where(p => p.Activo).ToList();

        // Obtener todos los CalidadParametros para optimizar las consultas
        var todosCalidadParametros = await _calidadParametroRepository.GetAllAsync();

        // Crear lista de resultados usando LEFT JOIN pattern
        var result = new List<CalidadParametroItemDTO>();

        foreach (var calidad in calidades)
        {
            // LEFT JOIN pattern: Catálogo de parámetros vs valores guardados
            // Esto garantiza que SIEMPRE se devuelvan todos los parámetros:
            // - Si existe valor en BD → usa el valor real
            // - Si NO existe valor en BD → usa valor por defecto (0) y marca EsDefault = true
            var parametrosConValores = from parametro in parametrosActivos
                                      join calidadParametro in todosCalidadParametros
                                          .Where(cp => cp.CalidadId == calidad.Id && cp.Activo)
                                          on parametro.Id equals calidadParametro.ParametroId into leftJoin
                                      from cp in leftJoin.DefaultIfEmpty()
                                      select new CalidadParametroItemDTO
                                      {
                                          Id = cp?.Id ?? Guid.NewGuid(), // ID temporal para registros por defecto
                                          CalidadId = calidad.Id,
                                          CalidadCodigo = calidad.Codigo,
                                          CalidadNombre = calidad.Nombre,
                                          ParametroId = parametro.Id,
                                          ParametroCodigo = parametro.Codigo,
                                          ParametroNombre = parametro.Nombre,
                                          Valor = cp?.Valor ?? 0, // Valor por defecto: 0
                                          EsDefault = cp == null, // true si no existe registro en BD
                                          Activo = cp?.Activo ?? true, // Activo por defecto para nuevos
                                          CreadoPorId = cp?.CreadoPorId ?? Guid.Empty,
                                          CreadoEl = cp?.CreadoEl ?? DateTime.UtcNow,
                                          ModificadoPorId = cp?.ModificadoPorId,
                                          ModificadoEl = cp?.ModificadoEl
                                      };

            result.AddRange(parametrosConValores);
        }

        // Ordenar por código de calidad y luego por código de parámetro
        return result
            .OrderBy(r => r.CalidadCodigo)
            .ThenBy(r => r.ParametroCodigo)
            .ToList();
    }
}
