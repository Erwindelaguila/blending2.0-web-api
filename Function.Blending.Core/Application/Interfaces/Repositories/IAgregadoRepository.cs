using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IAgregadoRepository
{
    Task<List<AgregadoEntity>> GetAllAsync();
    Task<AgregadoEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(AgregadoEntity agregado);
    Task UpdateAsync(AgregadoEntity agregado);
    Task DeleteAsync(Guid id);
}
