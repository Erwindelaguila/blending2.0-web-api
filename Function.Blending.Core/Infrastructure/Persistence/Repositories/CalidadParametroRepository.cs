using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class CalidadParametroRepository : ICalidadParametroRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public CalidadParametroRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CalidadParametroEntity>> GetAllAsync()
    {
        var models = await _context.CalidadParametro
            .AsNoTracking()
            .Where(x => x.Activo)
            .Include(x => x.Calidad)
            .Include(x => x.Parametro)
            .OrderBy(x => x.CreadoEl)
            .ToListAsync();
            
        return _mapper.Map<List<CalidadParametroEntity>>(models);
    }

    public async Task<CalidadParametroEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.CalidadParametro
            .AsNoTracking()
            .Include(x => x.Calidad)
            .Include(x => x.Parametro)
            .FirstOrDefaultAsync(x => x.Id == id && x.Activo);
            
        return model != null ? _mapper.Map<CalidadParametroEntity>(model) : null;
    }

    public async Task<CalidadParametroEntity?> GetByCalidadParametroAsync(Guid calidadId, Guid parametroId)
    {
        var model = await _context.CalidadParametro
            .AsNoTracking()
            .Include(x => x.Calidad)
            .Include(x => x.Parametro)
            .FirstOrDefaultAsync(x => x.CalidadId == calidadId && x.ParametroId == parametroId && x.Activo);
            
        return model != null ? _mapper.Map<CalidadParametroEntity>(model) : null;
    }

    public async Task<List<CalidadParametroEntity>> GetMatrizDataAsync()
    {
        var models = await _context.CalidadParametro
            .AsNoTracking()
            .Where(x => x.Activo)
            .Include(x => x.Calidad)
            .Include(x => x.Parametro)
            .ToListAsync();
            
        return _mapper.Map<List<CalidadParametroEntity>>(models);
    }

    public async Task CreateAsync(CalidadParametroEntity calidadParametroEntity)
    {
        var model = _mapper.Map<CalidadParametro>(calidadParametroEntity);
        model.Id = Guid.NewGuid();
        model.CreadoEl = DateTime.UtcNow;
        model.Activo = true;
        
        _context.CalidadParametro.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CalidadParametroEntity calidadParametroEntity)
    {
        var existingEntity = await _context.CalidadParametro
            .FirstOrDefaultAsync(x => x.Id == calidadParametroEntity.Id);
            
        if (existingEntity == null || !existingEntity.Activo)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está inactivo.");
        }

        var model = _mapper.Map<CalidadParametro>(calidadParametroEntity);
        model.ModificadoEl = DateTime.UtcNow;
        
        _context.CalidadParametro.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpsertAsync(Guid calidadId, Guid parametroId, decimal valor, Guid userId)
    {
        var existing = await _context.CalidadParametro
            .FirstOrDefaultAsync(x => x.CalidadId == calidadId && x.ParametroId == parametroId);

        if (existing != null)
        {
            // Update existing
            existing.Valor = valor;
            existing.Activo = true;
            existing.ModificadoPorId = userId;
            existing.ModificadoEl = DateTime.UtcNow;
        }
        else
        {
            // Create new
            var newEntity = new CalidadParametro
            {
                Id = Guid.NewGuid(),
                CalidadId = calidadId,
                ParametroId = parametroId,
                Valor = valor,
                Activo = true,
                CreadoPorId = userId,
                CreadoEl = DateTime.UtcNow
            };
            _context.CalidadParametro.Add(newEntity);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> UpsertBatchAsync(List<(Guid CalidadId, Guid ParametroId, decimal Valor)> cambios, Guid userId)
    {
        var processedCount = 0;

        foreach (var (calidadId, parametroId, valor) in cambios)
        {
            var existing = await _context.CalidadParametro
                .FirstOrDefaultAsync(x => x.CalidadId == calidadId && x.ParametroId == parametroId);

            if (existing != null)
            {
                // Update existing
                existing.Valor = valor;
                existing.Activo = true;
                existing.ModificadoPorId = userId;
                existing.ModificadoEl = DateTime.UtcNow;
            }
            else
            {
                // Create new
                var newEntity = new CalidadParametro
                {
                    Id = Guid.NewGuid(),
                    CalidadId = calidadId,
                    ParametroId = parametroId,
                    Valor = valor,
                    Activo = true,
                    CreadoPorId = userId,
                    CreadoEl = DateTime.UtcNow
                };
                _context.CalidadParametro.Add(newEntity);
            }
            processedCount++;
        }

        await _context.SaveChangesAsync();
        return processedCount;
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.CalidadParametro.FindAsync(id);
        if (entity != null)
        {
            entity.Activo = false;
            entity.ModificadoPorId = eliminadoPorId;
            entity.ModificadoEl = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
