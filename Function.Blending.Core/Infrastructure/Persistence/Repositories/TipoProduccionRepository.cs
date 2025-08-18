using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class TipoProduccionRepository : ITipoProduccionRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public TipoProduccionRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TipoProduccionEntity>> GetAllAsync()
    {
        var models = await _context.TipoProduccion
            .Where(t => !t.Eliminado) // Solo registros no eliminados
            .ToListAsync();
        var entities = _mapper.Map<List<TipoProduccionEntity>>(models);
        return entities;
    }

    public async Task<TipoProduccionEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.TipoProduccion
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && !t.Eliminado); // Solo si no está eliminado

        if (model == null)
            return null;

        var entity = _mapper.Map<TipoProduccionEntity>(model);
        return entity;
    }

    public async Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = _context.TipoProduccion.Where(t => t.Codigo == codigo && !t.Eliminado);
        
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<(IReadOnlyList<TipoProduccionEntity> Items, int Total)> GetPagedAsync(int page, int size)
    {
        var baseQuery = _context.TipoProduccion.AsNoTracking()
            .Where(x => !x.Eliminado)
            .OrderBy(x => x.CreadoEl);

        var total = await baseQuery.CountAsync();
        var models = await baseQuery.Skip((page - 1) * size).Take(size).ToListAsync();
        return (_mapper.Map<List<TipoProduccionEntity>>(models), total);
    }

    public async Task CreateAsync(TipoProduccionEntity tipoProduccionEntity)
    {
        var model = _mapper.Map<TipoProduccion>(tipoProduccionEntity);
        _context.TipoProduccion.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TipoProduccionEntity tipoProduccionEntity)
    {
        var model = _mapper.Map<TipoProduccion>(tipoProduccionEntity);
        _context.TipoProduccion.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task<TipoProduccionEntity> UpdateAndReturnAsync(TipoProduccionEntity tipoProduccionEntity)
    {
        // Obtener el registro actual para validar existencia y preservar datos
        var currentEntity = await _context.TipoProduccion
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tipoProduccionEntity.Id);
            
        if (currentEntity == null || currentEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(tipoProduccionEntity.Codigo, tipoProduccionEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        // Preservar campos que no deben modificarse
        tipoProduccionEntity.CreadoPorId = currentEntity.CreadoPorId;
        tipoProduccionEntity.CreadoEl = currentEntity.CreadoEl;

        var model = _mapper.Map<TipoProduccion>(tipoProduccionEntity);
        _context.TipoProduccion.Update(model);
        await _context.SaveChangesAsync();

        return _mapper.Map<TipoProduccionEntity>(model);
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.TipoProduccion.FindAsync(id);
        if (entity is null || entity.Eliminado) return;

        // Soft delete
        entity.Eliminado = true;
        entity.EliminadoPorId = eliminadoPorId;
        entity.EliminadoEl = DateTime.UtcNow;

        _context.TipoProduccion.Update(entity);
        await _context.SaveChangesAsync();
    }
}
