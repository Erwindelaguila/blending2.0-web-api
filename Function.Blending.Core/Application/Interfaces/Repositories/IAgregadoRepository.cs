using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IAgregadoRepository
{
    Task<List<AgregadoEntity>> GetAllAsync();
    Task<AgregadoEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    Task<(IReadOnlyList<AgregadoEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(AgregadoEntity agregado);
    Task UpdateAsync(AgregadoEntity agregado);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}
