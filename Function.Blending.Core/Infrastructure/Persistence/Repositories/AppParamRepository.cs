using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class AppParamRepository : IAppParamRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public AppParamRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AppParamEntity>> GetAllAsync()
    {
        var models = await _context.AppParam
            .ToListAsync();
        var entities = _mapper.Map<List<AppParamEntity>>(models);
        return entities;
    }

    public async Task<AppParamEntity?> GetByKeyAsync(string key)
    {
        var model = await _context.AppParam
            .AsNoTracking()
            .FirstOrDefaultAsync(ap => ap.Key == key);

        if (model == null)
            return null;

        var entity = _mapper.Map<AppParamEntity>(model);
        return entity;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _context.AppParam.AnyAsync(ap => ap.Key == key);
    }

    public IQueryable<AppParamEntity> GetQueryable()
    {
        return _context.AppParam.Select(ap => new AppParamEntity
        {
            Key = ap.Key,
            Value = ap.Value,
            Description = ap.Description,
            Category = ap.Category,
            Group = ap.Group,
            IsActive = ap.IsActive,
            IsInternal = ap.IsInternal,
            IsVisible = ap.IsVisible,
            IsDisableable = ap.IsDisableable,
            IsRemovable = ap.IsRemovable,
            CreadoPorId = ap.CreadoPorId,
            CreadoEl = ap.CreadoEl,
            ModificadoPorId = ap.ModificadoPorId,
            ModificadoEl = ap.ModificadoEl
        });
    }

    public async Task<(IReadOnlyList<AppParamEntity> Items, int Total)> GetPagedAsync(int page, int size)
    {
        var skip = (page - 1) * size;
        
        var query = _context.AppParam.AsQueryable();
        
        var total = await query.CountAsync();
        
        var models = await query
            .Skip(skip)
            .Take(size)
            .ToListAsync();
        
        var entities = _mapper.Map<List<AppParamEntity>>(models);
        
        return (entities, total);
    }

    public async Task CreateAsync(AppParamEntity appParam)
    {
        var model = _mapper.Map<AppParam>(appParam);
        _context.AppParam.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task<AppParamEntity> CreateAndReturnAsync(AppParamEntity appParam)
    {
        var model = _mapper.Map<AppParam>(appParam);
        _context.AppParam.Add(model);
        await _context.SaveChangesAsync();
        
        var createdEntity = _mapper.Map<AppParamEntity>(model);
        return createdEntity;
    }

    public async Task UpdateAsync(AppParamEntity appParam)
    {
        var model = _mapper.Map<AppParam>(appParam);
        _context.AppParam.Update(model);
        await _context.SaveChangesAsync();
    }

    public async Task<AppParamEntity> UpdateAndReturnAsync(AppParamEntity appParam)
    {
        var model = _mapper.Map<AppParam>(appParam);
        _context.AppParam.Update(model);
        await _context.SaveChangesAsync();
        
        var updatedEntity = _mapper.Map<AppParamEntity>(model);
        return updatedEntity;
    }

    public async Task DeleteAsync(string key)
    {
        var model = await _context.AppParam.FirstOrDefaultAsync(ap => ap.Key == key);
        if (model != null)
        {
            _context.AppParam.Remove(model);
            await _context.SaveChangesAsync();
        }
    }
}
