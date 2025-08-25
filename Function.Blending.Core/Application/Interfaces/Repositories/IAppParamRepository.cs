using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IAppParamRepository
{
    Task<List<AppParamEntity>> GetAllAsync();
    Task<AppParamEntity?> GetByKeyAsync(string key);
    Task<List<AppParamEntity>> GetByCategoryAsync(string category);
    Task<List<AppParamEntity>> GetByGroupAsync(string group);
    Task<List<AppParamEntity>> GetByCategoryAndGroupAsync(string category, string group);
    Task<bool> ExistsAsync(string key);
    IQueryable<AppParamEntity> GetQueryable();
    Task<(IReadOnlyList<AppParamEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(AppParamEntity appParam);
    Task UpdateAsync(AppParamEntity appParam);
    Task<AppParamEntity> UpdateAndReturnAsync(AppParamEntity appParam);
    Task DeleteAsync(string key);
}
