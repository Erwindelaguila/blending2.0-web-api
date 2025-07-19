using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ITipoProduccionRepository
{
    Task<List<TipoProduccionEntity>> GetAllAsync();
    Task<TipoProduccionEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(TipoProduccionEntity tipo);
    Task UpdateAsync(TipoProduccionEntity tipo);
    Task DeleteAsync(Guid id);
}
