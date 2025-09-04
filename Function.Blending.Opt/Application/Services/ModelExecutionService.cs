using AutoMapper;
using Function.Blending.Opt.Infrastructure.Data;

namespace Function.Blending.Opt.Application.Services;

public class ModelExecutionService
{
    private readonly BlendingDbContext _db;
    private readonly IMapper _mapper;

    public ModelExecutionService(BlendingDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Guid> RegistrarEjecucionAsync(string planta, string tipoModelo)
    {
        var ejecucionDominio = new Domain.Entities.ModelExecution
        {
            Planta = planta,
            TipoModelo = tipoModelo
        };

        var ejecucionEF = _mapper.Map<Infrastructure.Data.Scaffolded.ModelExecution>(ejecucionDominio);

        Console.WriteLine($"[DEBUG] Insertando ModelExecution EF: {ejecucionEF.Planta}, {ejecucionEF.TipoModelo}");

        _db.ModelExecutions.Add(ejecucionEF);
        await _db.SaveChangesAsync();

        Console.WriteLine($"[DEBUG] Registro insertado con ID: {ejecucionEF.Id}");

        return ejecucionEF.Id;
    }

    public async Task MarkExecutionAsCompletedAsync(string executionId, string mensaje, DateTime timestamp)
    {
        var guid = Guid.Parse(executionId);
        var model = await _db.ModelExecutions.FindAsync(guid);

        if (model == null)
            throw new InvalidOperationException($"No se encontró la ejecución con ID: {executionId}");

        model.Estado = "Completado";
        model.FechaFin = timestamp;
        model.MensajeResultado = mensaje;

        await _db.SaveChangesAsync();
    }
}

