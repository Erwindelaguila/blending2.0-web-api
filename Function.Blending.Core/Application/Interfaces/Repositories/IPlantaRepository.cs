using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IPlantaRepository
{
    Task<IEnumerable<Planta>> GetAllAsync();
    Task<Planta?> GetByIdAsync(Guid id);
    Task AddAsync(Planta planta);
    Task UpdateAsync(Planta planta);
    Task DeleteAsync(Guid id);
}