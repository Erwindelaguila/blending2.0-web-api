using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class CalidadRepository : ICalidadRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public CalidadRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CalidadEntity>> GetAllAsync()
    {
        var models = await _context.Calidad
            .Where(c => !c.Eliminado) // Solo registros no eliminados
            .ToListAsync();
        var entities = _mapper.Map<List<CalidadEntity>>(models);
        return entities;
    }

    public async Task<CalidadEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Calidad
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.Eliminado); // Solo si no está eliminado

        if (model == null)
            return null;

        var entity = _mapper.Map<CalidadEntity>(model);
        return entity;
    }

    public async Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = _context.Calidad.Where(c => c.Codigo == codigo && !c.Eliminado);
        
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public IQueryable<CalidadEntity> GetQueryable()
    {
        return _context.Calidad
            .Where(x => !x.Eliminado)
            .Select(c => new CalidadEntity
            {
                Id = c.Id,
                Codigo = c.Codigo,
                Nombre = c.Nombre,
                CodigoMaterial = c.CodigoMaterial,
                Descripcion = c.Descripcion,
                NoConforme = c.NoConforme,
                Activo = c.Activo,
                CreadoPorId = c.CreadoPorId,
                CreadoEl = c.CreadoEl,
                ModificadoPorId = c.ModificadoPorId,
                ModificadoEl = c.ModificadoEl,
                Eliminado = c.Eliminado,
                EliminadoPorId = c.EliminadoPorId,
                EliminadoEl = c.EliminadoEl
            });
            // Quito el OrderBy para manejarlo en el handler según los filtros aplicados
    }

    public async Task<(IReadOnlyList<CalidadEntity> Items, int Total)> GetPagedAsync(int page, int size)
    {
        var baseQuery = _context.Calidad.AsNoTracking()
            .Where(x => !x.Eliminado)
            .OrderBy(x => x.CreadoEl);

        var total = await baseQuery.CountAsync();
        var models = await baseQuery.Skip((page - 1) * size).Take(size).ToListAsync();
        return (_mapper.Map<List<CalidadEntity>>(models), total);
    }

    public async Task CreateAsync(CalidadEntity calidadEntity)
    {
        // Validar que el código no exista entre registros activos
        if (await ExistsActiveCodigoAsync(calidadEntity.Codigo))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Calidad>(calidadEntity);
        _context.Calidad.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CalidadEntity calidadEntity)
    {
        // Verificar que el registro existe y no está eliminado (usando AsNoTracking para evitar tracking)
        var existingEntity = await _context.Calidad
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == calidadEntity.Id);
            
        if (existingEntity == null || existingEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(calidadEntity.Codigo, calidadEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        var model = _mapper.Map<Calidad>(calidadEntity);
        _context.Calidad.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task<CalidadEntity> UpdateAndReturnAsync(CalidadEntity calidadEntity)
    {
        // Obtener el registro actual para validar existencia y preservar datos
        var currentEntity = await _context.Calidad
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == calidadEntity.Id);
            
        if (currentEntity == null || currentEntity.Eliminado)
        {
            throw new InvalidOperationException("No se puede modificar un registro que no existe o está eliminado.");
        }

        // Validar que el código no exista entre otros registros activos
        if (await ExistsActiveCodigoAsync(calidadEntity.Codigo, calidadEntity.Id))
        {
            throw new ArgumentException("Código duplicado", "codigo");
        }

        // Preservar campos que no deben modificarse
        calidadEntity.CreadoPorId = currentEntity.CreadoPorId;
        calidadEntity.CreadoEl = currentEntity.CreadoEl;
        calidadEntity.Eliminado = currentEntity.Eliminado;
        calidadEntity.EliminadoPorId = currentEntity.EliminadoPorId;
        calidadEntity.EliminadoEl = currentEntity.EliminadoEl;

        var model = _mapper.Map<Calidad>(calidadEntity);
        _context.Calidad.Update(model);
        await _context.SaveChangesAsync();

        // Retornar la entidad actualizada
        return calidadEntity;
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.Calidad.FindAsync(id);
        if (entity is null || entity.Eliminado) return;

        // Soft delete
        entity.Eliminado = true;
        entity.EliminadoPorId = eliminadoPorId;
        entity.EliminadoEl = DateTime.UtcNow;

        _context.Calidad.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUsedByActiveProductoAsync(Guid calidadId)
    {
        return await _context.Producto
            .AnyAsync(p => p.CalidadId == calidadId && 
                          p.Activo && 
                          !p.Eliminado);
    }

    public async Task<List<CalidadActivaDTO>> GetActivasAsync()
    {
        return await _context.Calidad
            .Where(c => c.Activo && !c.Eliminado)
            .Select(c => new CalidadActivaDTO
            {
                Id = c.Id,
                Codigo = c.Codigo
            })
            .ToListAsync();
    }
}