using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IPlantaRepository
{
    Task<List<PlantaEntity>> GetAllAsync();
    Task<PlantaEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(PlantaEntity plantaEntity);
    Task UpdateAsync(PlantaEntity plantaEntity);
    Task DeleteAsync(Guid id);
}