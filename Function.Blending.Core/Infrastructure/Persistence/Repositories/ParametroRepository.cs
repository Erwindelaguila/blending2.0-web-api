using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class ParametroRepository : IParametroRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public ParametroRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ParametroEntity>> GetAllAsync()
    {
        var models = await _context.Parametro
            .Where(p => !p.Eliminado) // Solo registros no eliminados
            .ToListAsync();
        var entities = _mapper.Map<List<ParametroEntity>>(models);
        return entities;
    }

    public async Task<ParametroEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Parametro
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado); // Solo si no está eliminado

        if (model == null)
            return null;

        var entity = _mapper.Map<ParametroEntity>(model);
        return entity;
    }

    public async Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = _context.Parametro.Where(p => p.Codigo == codigo && !p.Eliminado);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public IQueryable<ParametroEntity> GetQueryable()
    {
        return _context.Parametro
            .Where(x => !x.Eliminado)
            .Select(p => new ParametroEntity
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Activo = p.Activo,
                CreadoPorId = p.CreadoPorId,
                CreadoEl = p.CreadoEl,
                ModificadoPorId = p.ModificadoPorId,
                ModificadoEl = p.ModificadoEl,
                Eliminado = p.Eliminado,
                EliminadoPorId = p.EliminadoPorId,
                EliminadoEl = p.EliminadoEl
            });
            // Quito el OrderBy para manejarlo en el handler según los filtros aplicados
    }

    public async Task<(IReadOnlyList<ParametroEntity> Items, int Total)> GetPagedAsync(int page, int size)
    {
        var baseQuery = _context.Parametro.AsNoTracking()
            .Where(x => !x.Eliminado)
            .OrderBy(x => x.CreadoEl);

        var total = await baseQuery.CountAsync();
        var models = await baseQuery.Skip((page - 1) * size).Take(size).ToListAsync();
        return (_mapper.Map<List<ParametroEntity>>(models), total);
    }

    public async Task CreateAsync(ParametroEntity parametroEntity)
    {
        // Validar que el código no exista entre registros activos
        if (await ExistsActiveCodigoAsync(parametroEntity.Codigo))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Parametro>(parametroEntity);
        _context.Parametro.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParametroEntity parametroEntity)
    {
        // Verificar que el registro existe y no está eliminado (usando AsNoTracking para evitar tracking)
        var existingEntity = await _context.Parametro
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == parametroEntity.Id);
            
        if (existingEntity == null || existingEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(parametroEntity.Codigo, parametroEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Parametro>(parametroEntity);
        _context.Parametro.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task<ParametroEntity> UpdateAndReturnAsync(ParametroEntity parametroEntity)
    {
        // Obtener el registro actual para validar existencia y preservar datos
        var currentEntity = await _context.Parametro
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == parametroEntity.Id);
            
        if (currentEntity == null || currentEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(parametroEntity.Codigo, parametroEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        // Preservar campos que no deben modificarse
        parametroEntity.CreadoPorId = currentEntity.CreadoPorId;
        parametroEntity.CreadoEl = currentEntity.CreadoEl;
        parametroEntity.Eliminado = currentEntity.Eliminado;
        parametroEntity.EliminadoPorId = currentEntity.EliminadoPorId;
        parametroEntity.EliminadoEl = currentEntity.EliminadoEl;

        var model = _mapper.Map<Parametro>(parametroEntity);
        _context.Parametro.Update(model);
        await _context.SaveChangesAsync();

        // Retornar la entidad actualizada
        return parametroEntity;
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.Parametro.FindAsync(id);
        if (entity is null || entity.Eliminado) return;

        // Soft delete
        entity.Eliminado = true;
        entity.EliminadoPorId = eliminadoPorId;
        entity.EliminadoEl = DateTime.UtcNow;

        _context.Parametro.Update(entity);
        await _context.SaveChangesAsync();
    }
}
