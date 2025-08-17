using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class LineaProduccionRepository : ILineaProduccionRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public LineaProduccionRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LineaProduccionEntity>> GetAllAsync()
    {
        var models = await _context.LineaProduccion
            .Where(l => !l.Eliminado) // Solo registros no eliminados
            .ToListAsync();
        var entities = _mapper.Map<List<LineaProduccionEntity>>(models);
        return entities;
    }

    public async Task<LineaProduccionEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.LineaProduccion
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id && !l.Eliminado); // Solo si no está eliminado

        if (model == null)
            return null;

        var entity = _mapper.Map<LineaProduccionEntity>(model);
        return entity;
    }

    public async Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = _context.LineaProduccion.Where(l => l.Codigo == codigo && !l.Eliminado);
        
        if (excludeId.HasValue)
            query = query.Where(l => l.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public IQueryable<LineaProduccionEntity> GetQueryable()
    {
        return _context.LineaProduccion
            .Where(x => !x.Eliminado)
            .Select(l => new LineaProduccionEntity
            {
                Id = l.Id,
                Codigo = l.Codigo,
                Nombre = l.Nombre,
                Descripcion = l.Descripcion,
                Activo = l.Activo,
                CreadoPorId = l.CreadoPorId,
                CreadoEl = l.CreadoEl,
                ModificadoPorId = l.ModificadoPorId,
                ModificadoEl = l.ModificadoEl,
                Eliminado = l.Eliminado,
                EliminadoPorId = l.EliminadoPorId,
                EliminadoEl = l.EliminadoEl
            });
            // Quito el OrderBy para manejarlo en el handler según los filtros aplicados
    }

    public async Task<(IReadOnlyList<LineaProduccionEntity> Items, int Total)> GetPagedAsync(int page, int size)
    {
        var baseQuery = _context.LineaProduccion.AsNoTracking()
            .Where(x => !x.Eliminado)
            .OrderBy(x => x.CreadoEl);

        var total = await baseQuery.CountAsync();
        var models = await baseQuery.Skip((page - 1) * size).Take(size).ToListAsync();
        return (_mapper.Map<List<LineaProduccionEntity>>(models), total);
    }

    public async Task CreateAsync(LineaProduccionEntity lineaProduccionEntity)
    {
        // Validar que el código no exista entre registros activos
        if (await ExistsActiveCodigoAsync(lineaProduccionEntity.Codigo))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<LineaProduccion>(lineaProduccionEntity);
        _context.LineaProduccion.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LineaProduccionEntity lineaProduccionEntity)
    {
        // Verificar que el registro existe y no está eliminado (usando AsNoTracking para evitar tracking)
        var existingEntity = await _context.LineaProduccion
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lineaProduccionEntity.Id);
            
        if (existingEntity == null || existingEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(lineaProduccionEntity.Codigo, lineaProduccionEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<LineaProduccion>(lineaProduccionEntity);
        _context.LineaProduccion.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task<LineaProduccionEntity> UpdateAndReturnAsync(LineaProduccionEntity lineaProduccionEntity)
    {
        // Obtener el registro actual para validar existencia y preservar datos
        var currentEntity = await _context.LineaProduccion
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lineaProduccionEntity.Id);
            
        if (currentEntity == null || currentEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(lineaProduccionEntity.Codigo, lineaProduccionEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        // Preservar campos que no deben modificarse
        lineaProduccionEntity.CreadoPorId = currentEntity.CreadoPorId;
        lineaProduccionEntity.CreadoEl = currentEntity.CreadoEl;
        lineaProduccionEntity.Eliminado = currentEntity.Eliminado;
        lineaProduccionEntity.EliminadoPorId = currentEntity.EliminadoPorId;
        lineaProduccionEntity.EliminadoEl = currentEntity.EliminadoEl;

        var model = _mapper.Map<LineaProduccion>(lineaProduccionEntity);
        _context.LineaProduccion.Update(model);
        await _context.SaveChangesAsync();

        // Retornar la entidad actualizada
        return lineaProduccionEntity;
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.LineaProduccion.FindAsync(id);
        if (entity is null || entity.Eliminado) return;

        // Soft delete
        entity.Eliminado = true;
        entity.EliminadoPorId = eliminadoPorId;
        entity.EliminadoEl = DateTime.UtcNow;

        _context.LineaProduccion.Update(entity);
        await _context.SaveChangesAsync();
    }
}
