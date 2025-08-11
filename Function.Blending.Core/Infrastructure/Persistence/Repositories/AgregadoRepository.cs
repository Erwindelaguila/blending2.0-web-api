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
            .ToListAsync();
        var entities = _mapper.Map<List<AgregadoEntity>>(models);
        return entities;
    }

    public async Task<AgregadoEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Agregado
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (model == null)
            return null;

        var entity = _mapper.Map<AgregadoEntity>(model);
        return entity;
    }

    public async Task CreateAsync(AgregadoEntity agregadoEntity)
    {
        var model = _mapper.Map<Agregado>(agregadoEntity);
        _context.Agregado.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AgregadoEntity agregadoEntity)
    {
        var model = _mapper.Map<Agregado>(agregadoEntity);
        _context.Agregado.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Agregado.FindAsync(id);
        if (entity is null) return;

        _context.Agregado.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
