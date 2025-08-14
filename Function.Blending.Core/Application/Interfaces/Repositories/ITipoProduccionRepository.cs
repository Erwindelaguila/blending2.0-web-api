using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ITipoProduccionRepository
{
    Task<List<TipoProduccionEntity>> GetAllAsync();
    Task<TipoProduccionEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    Task<(IReadOnlyList<TipoProduccionEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(TipoProduccionEntity tipo);
    Task UpdateAsync(TipoProduccionEntity tipo);
    Task<TipoProduccionEntity> UpdateAndReturnAsync(TipoProduccionEntity tipo);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}
