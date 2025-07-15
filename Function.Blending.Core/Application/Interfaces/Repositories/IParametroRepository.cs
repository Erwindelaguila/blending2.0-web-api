using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IParametroRepository
{
    Task<List<ParametroEntity>> GetAllAsync();
    Task<ParametroEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(ParametroEntity parametro);
    Task UpdateAsync(ParametroEntity parametro);
    Task DeleteAsync(Guid id);
}
