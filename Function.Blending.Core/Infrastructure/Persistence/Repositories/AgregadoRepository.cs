using AutoMapper;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class AgregadoRepository : IAgregadoRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public AgregadoRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AgregadoEntity>> GetAllAsync()
    {
        var models = await _context.Agregado
            .Where(a => !a.Eliminado) // Solo registros no eliminados
            .ToListAsync();
        var entities = _mapper.Map<List<AgregadoEntity>>(models);
        return entities;
    }

    public async Task<AgregadoEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Agregado
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado); // Solo si no está eliminado

        if (model == null)
            return null;

        var entity = _mapper.Map<AgregadoEntity>(model);
        return entity;
    }

    public async Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = _context.Agregado.Where(a => a.Codigo == codigo && !a.Eliminado);
        
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public IQueryable<AgregadoEntity> GetQueryable()
    {
        return _context.Agregado
            .Where(x => !x.Eliminado)
            .Select(a => new AgregadoEntity
            {
                Id = a.Id,
                Codigo = a.Codigo,
                Nombre = a.Nombre,
                Descripcion = a.Descripcion,
                Activo = a.Activo,
                CreadoPorId = a.CreadoPorId,
                CreadoEl = a.CreadoEl,
                ModificadoPorId = a.ModificadoPorId,
                ModificadoEl = a.ModificadoEl,
                Eliminado = a.Eliminado,
                EliminadoPorId = a.EliminadoPorId,
                EliminadoEl = a.EliminadoEl
            });
            // Quito el OrderBy para manejarlo en el handler según los filtros aplicados
    }

    public async Task CreateAsync(AgregadoEntity agregadoEntity)
    {
        // Validar que el código no exista entre registros activos
        if (await ExistsActiveCodigoAsync(agregadoEntity.Codigo))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Agregado>(agregadoEntity);
        _context.Agregado.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AgregadoEntity agregadoEntity)
    {
        // Verificar que el registro existe y no está eliminado (usando AsNoTracking para evitar tracking)
        var existingEntity = await _context.Agregado
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == agregadoEntity.Id);
            
        if (existingEntity == null || existingEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(agregadoEntity.Codigo, agregadoEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Agregado>(agregadoEntity);
        _context.Agregado.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.Agregado.FindAsync(id);
        if (entity is null || entity.Eliminado) return;

        // Soft delete
        entity.Eliminado = true;
        entity.EliminadoPorId = eliminadoPorId;
        entity.EliminadoEl = DateTime.UtcNow;

        _context.Agregado.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<AgregadoActivoDTO>> GetActivosAsync()
    {
        return await _context.Agregado
            .Where(a => !a.Eliminado && a.Activo) // Solo registros activos (no eliminados Y activo = true)
            .Select(a => new AgregadoActivoDTO 
            { 
                Id = a.Id, 
                Codigo = a.Codigo 
            })
            .ToListAsync();
    }

    public async Task<int> GetActiveTipoProduccionCountAsync(Guid agregadoId)
    {
        return await _context.TipoProduccion
            .Where(tp => tp.AgregadoId == agregadoId && !tp.Eliminado && tp.Activo)
            .CountAsync();
    }

    public async Task<bool> IsUsedByActiveTipoProduccionAsync(Guid agregadoId)
    {
        return await _context.TipoProduccion
            .AnyAsync(tp => tp.AgregadoId == agregadoId && !tp.Eliminado && tp.Activo);
    }
}
