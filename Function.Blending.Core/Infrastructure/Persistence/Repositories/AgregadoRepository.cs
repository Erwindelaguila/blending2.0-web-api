using AutoMapper;
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

    public async Task<(IReadOnlyList<AgregadoEntity> Items, int Total)> GetPagedAsync(int page, int size)
    {
        var baseQuery = _context.Agregado.AsNoTracking()
            .Where(x => !x.Eliminado)
            .OrderBy(x => x.CreadoEl);

        var total = await baseQuery.CountAsync();
        var models = await baseQuery.Skip((page - 1) * size).Take(size).ToListAsync();
        return (_mapper.Map<List<AgregadoEntity>>(models), total);
    }

    public async Task CreateAsync(AgregadoEntity agregadoEntity)
    {
        // Validar que el código no exista entre registros activos
        if (await ExistsActiveCodigoAsync(agregadoEntity.Codigo))
        {
            throw new ArgumentException($"DUPLICATE_CODE|{agregadoEntity.Codigo}", "codigo");
        }

        var model = _mapper.Map<Agregado>(agregadoEntity);
        _context.Agregado.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AgregadoEntity agregadoEntity)
    {
        // Verificar que el registro existe y no está eliminado
        var existingEntity = await _context.Agregado.FindAsync(agregadoEntity.Id);
        if (existingEntity == null || existingEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(agregadoEntity.Codigo, agregadoEntity.Id))
        {
            throw new ArgumentException($"DUPLICATE_CODE|{agregadoEntity.Codigo}", "codigo");
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
}
