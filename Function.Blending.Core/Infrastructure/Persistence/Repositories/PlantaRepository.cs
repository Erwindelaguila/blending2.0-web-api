using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class PlantaRepository : IPlantaRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public PlantaRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<PlantaEntity>> GetAllAsync()
    {
        var models = await _context.Planta
            .Where(p => !p.Eliminado) // Solo registros no eliminados
            .ToListAsync();
        var entities = _mapper.Map<List<PlantaEntity>>(models);
        return entities;
    }

    public async Task<PlantaEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Planta
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado); // Solo si no está eliminado

        if (model == null)
            return null;

        var entity = _mapper.Map<PlantaEntity>(model);
        return entity;
    }

    public async Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = _context.Planta.Where(p => p.Codigo == codigo && !p.Eliminado);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<(IReadOnlyList<PlantaEntity> Items, int Total)> GetPagedAsync(int page, int size)
    {
        var baseQuery = _context.Planta.AsNoTracking()
            .Where(x => !x.Eliminado)
            .OrderBy(x => x.CreadoEl);

        var total = await baseQuery.CountAsync();
        var models = await baseQuery.Skip((page - 1) * size).Take(size).ToListAsync();
        return (_mapper.Map<List<PlantaEntity>>(models), total);
    }

    public async Task CreateAsync(PlantaEntity plantaEntity)
    {
        // Validar que el código no exista entre registros activos
        if (await ExistsActiveCodigoAsync(plantaEntity.Codigo))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Planta>(plantaEntity);
        _context.Planta.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PlantaEntity plantaEntity)
    {
        // Verificar que el registro existe y no está eliminado (usando AsNoTracking para evitar tracking)
        var existingEntity = await _context.Planta
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == plantaEntity.Id);
            
        if (existingEntity == null || existingEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(plantaEntity.Codigo, plantaEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Planta>(plantaEntity);
        _context.Planta.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task<PlantaEntity> UpdateAndReturnAsync(PlantaEntity plantaEntity)
    {
        // Obtener el registro actual para validar existencia y preservar datos
        var currentEntity = await _context.Planta
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == plantaEntity.Id);
            
        if (currentEntity == null || currentEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(plantaEntity.Codigo, plantaEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        // Preservar campos que no deben modificarse
        plantaEntity.CreadoPorId = currentEntity.CreadoPorId;
        plantaEntity.CreadoEl = currentEntity.CreadoEl;
        plantaEntity.Eliminado = currentEntity.Eliminado;
        plantaEntity.EliminadoPorId = currentEntity.EliminadoPorId;
        plantaEntity.EliminadoEl = currentEntity.EliminadoEl;

        var model = _mapper.Map<Planta>(plantaEntity);
        _context.Planta.Update(model);
        await _context.SaveChangesAsync();

        // Retornar la entidad actualizada
        return plantaEntity;
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.Planta.FindAsync(id);
        if (entity is null || entity.Eliminado) return;

        // Soft delete
        entity.Eliminado = true;
        entity.EliminadoPorId = eliminadoPorId;
        entity.EliminadoEl = DateTime.UtcNow;

        _context.Planta.Update(entity);
        await _context.SaveChangesAsync();
    }
}
