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
        var models = await _context.TipoProduccions
        .ToListAsync();
        var entities = _mapper.Map<List<TipoProduccionEntity>>(models);
        return entities;
    }

    public async Task<TipoProduccionEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.TipoProduccions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (model == null)
            return null;

        var entity = _mapper.Map<TipoProduccionEntity>(model);
        return entity;
    }

    public async Task CreateAsync(TipoProduccionEntity tipoProduccionEntity)
    {
        var model = _mapper.Map<TipoProduccion>(tipoProduccionEntity);
        _context.TipoProduccions.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TipoProduccionEntity tipoProduccionEntity)
    {
        var model = _mapper.Map<TipoProduccion>(tipoProduccionEntity);
        _context.TipoProduccions.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.TipoProduccions.FindAsync(id);
        if (entity is null) return;

        _context.TipoProduccions.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
