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
        var models = await _context.LineaProduccions.ToListAsync();
        var entities = _mapper.Map<List<LineaProduccionEntity>>(models);
        return entities;
    }

    public async Task<LineaProduccionEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.LineaProduccions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (model == null)
            return null;
        var entity = _mapper.Map<LineaProduccionEntity>(model);
        return entity;
    }

    public async Task CreateAsync(LineaProduccionEntity entity)
    {
        var model = _mapper.Map<LineaProduccion>(entity);
        _context.LineaProduccions.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(LineaProduccionEntity entity)
    {
        var model = _mapper.Map<LineaProduccion>(entity);
        _context.LineaProduccions.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.LineaProduccions.FindAsync(id);
        if (entity is null) return;
        _context.LineaProduccions.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
