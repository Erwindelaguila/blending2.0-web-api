using AutoMapper;
using AutoMapper.QueryableExtensions;
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

    public async Task<IEnumerable<Planta>> GetAllAsync()
    {
        return await _context.Planta
            .Where(p => p.Activo)
            .ProjectTo<Planta>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Planta?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Planta.FindAsync(id);
        return entity is null ? null : _mapper.Map<Planta>(entity);
    }

    public async Task AddAsync(Planta planta)
    {
        var model = _mapper.Map<Plantum>(planta);
        _context.Planta.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Planta planta)
    {
        var model = await _context.Planta.FindAsync(planta.Id);
        if (model is null) return;

        _mapper.Map(planta, model);
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
