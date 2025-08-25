using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IAgregadoRepository
{
    Task<List<AgregadoEntity>> GetAllAsync();
    Task<AgregadoEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    IQueryable<AgregadoEntity> GetQueryable();
    Task CreateAsync(AgregadoEntity agregado);
    Task UpdateAsync(AgregadoEntity agregado);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
    Task<List<AgregadoActivoDTO>> GetActivosAsync();
    Task<int> GetActiveTipoProduccionCountAsync(Guid agregadoId);
    Task<bool> IsUsedByActiveTipoProduccionAsync(Guid agregadoId);
}
