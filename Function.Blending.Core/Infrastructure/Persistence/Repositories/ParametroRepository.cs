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
        var models = await _context.Parametros
            .ToListAsync();
        var entities = _mapper.Map<List<ParametroEntity>>(models);
        return entities;
    }

    public async Task<ParametroEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Parametros
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (model == null)
            return null;

        var entity = _mapper.Map<ParametroEntity>(model);
        return entity;
    }

    public async Task CreateAsync(ParametroEntity parametroEntity)
    {
        var model = _mapper.Map<Parametro>(parametroEntity);
        _context.Parametros.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ParametroEntity parametroEntity)
    {
        var model = _mapper.Map<Parametro>(parametroEntity);
        _context.Parametros.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.Parametros.FindAsync(id);
        if (entity is null) return;

        _context.Parametros.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
