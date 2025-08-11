using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class CalidadRepository: ICalidadRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public CalidadRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task CreateAsync(CalidadEntity calidadEntity)
    {
        var model = _mapper.Map<Calidad>(calidadEntity);
        _context.Calidad.Add(model);
        await _context.SaveChangesAsync();
    }
    public async Task<List<CalidadEntity>> GetAllAsync()
    {
        var models = await _context.Calidad
            .ToListAsync();
        var entities = _mapper.Map<List<CalidadEntity>>(models);
        return entities;
    }

    public async Task<CalidadEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Calidad
            .AsNoTracking() 
            .FirstOrDefaultAsync(c => c.Id == id);

        if (model == null)
            return null;

        var entity = _mapper.Map<CalidadEntity>(model);
        return entity;
    }
    
    public async Task UpdateAsync(CalidadEntity calidadEntity)
    {
        var model = _mapper.Map<Calidad>(calidadEntity);
        _context.Calidad.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Calidad.FindAsync(id);
        if (entity is null) return;

        _context.Calidad.Remove(entity);
        await _context.SaveChangesAsync();
    }

}