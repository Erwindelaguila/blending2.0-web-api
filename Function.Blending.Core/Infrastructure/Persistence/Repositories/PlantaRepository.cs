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
            .ToListAsync();
        var entities = _mapper.Map<List<PlantaEntity>>(models);
        return entities;
    }

    public async Task<PlantaEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Planta
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (model == null)
            return null;

        var entity = _mapper.Map<PlantaEntity>(model);
        return entity;
    }

    public async Task CreateAsync(PlantaEntity plantaEntity)
    {
        var model = _mapper.Map<Planta>(plantaEntity);
        _context.Planta.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PlantaEntity plantaEntity)
    {
        var model = _mapper.Map<Planta>(plantaEntity);
        _context.Planta.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Planta.FindAsync(id);
        if (entity is null) return;

        _context.Planta.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
