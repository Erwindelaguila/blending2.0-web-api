using Function.Blending.Core.Application.CalidadParametro.DTOs;
using Function.Blending.Core.Application.CalidadParametro.Queries;
using Function.Blending.Core.Application.Interfaces.Repositories;
using MediatR;

namespace Function.Blending.Core.Application.CalidadParametro.Handlers;

public class GetCalidadParametroMatrizQueryHandler : IRequestHandler<GetCalidadParametroMatrizQuery, CalidadParametroMatrizDTO>
{
    private readonly ICalidadRepository _calidadRepository;
    private readonly IParametroRepository _parametroRepository;
    private readonly ICalidadParametroRepository _calidadParametroRepository;

    public GetCalidadParametroMatrizQueryHandler(
        ICalidadRepository calidadRepository,
        IParametroRepository parametroRepository,
        ICalidadParametroRepository calidadParametroRepository)
    {
        _calidadRepository = calidadRepository;
        _parametroRepository = parametroRepository;
        _calidadParametroRepository = calidadParametroRepository;
    }

    public async Task<CalidadParametroMatrizDTO> Handle(GetCalidadParametroMatrizQuery request, CancellationToken cancellationToken)
    {
        // Obtener todas las calidades activas
        var calidades = await _calidadRepository.GetAllAsync();
        var calidadesActivas = calidades.Where(c => c.Activo).OrderBy(c => c.Codigo).ToList();

        // Obtener todos los parámetros activos
        var parametros = await _parametroRepository.GetAllAsync();
        var parametrosActivos = parametros.Where(p => p.Activo).OrderBy(p => p.Codigo).ToList();

        // Obtener todos los valores existentes
        var valoresExistentes = await _calidadParametroRepository.GetMatrizDataAsync();

        // Construir la matriz
        var matriz = new CalidadParametroMatrizDTO
        {
            Parametros = parametrosActivos.Select(p => new ParametroInfo
            {
                Id = p.Id,
                Codigo = p.Codigo
            }).ToList(),
            
            Calidades = calidadesActivas.Select(c => new CalidadInfo
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Valores = parametrosActivos.ToDictionary(
                    p => p.Codigo,
                    p =>
                    {
                        var valorExistente = valoresExistentes.FirstOrDefault(v => 
                            v.CalidadId == c.Id && v.ParametroId == p.Id);
                        
                        return new ValorInfo
                        {
                            Valor = valorExistente?.Valor ?? 0,
                            EsDefault = valorExistente == null
                        };
                    })
            }).ToList()
        };

        return matriz;
    }
}
