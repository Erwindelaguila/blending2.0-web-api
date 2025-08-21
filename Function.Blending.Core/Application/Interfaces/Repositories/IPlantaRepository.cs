using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IPlantaRepository
{
    Task<List<PlantaEntity>> GetAllAsync();
    Task<PlantaEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    IQueryable<PlantaEntity> GetQueryable();
    Task<(IReadOnlyList<PlantaEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(PlantaEntity planta);
    Task UpdateAsync(PlantaEntity planta);
    Task<PlantaEntity> UpdateAndReturnAsync(PlantaEntity planta);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}