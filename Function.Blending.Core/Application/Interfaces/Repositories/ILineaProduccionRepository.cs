using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ILineaProduccionRepository
{
    Task<List<LineaProduccionEntity>> GetAllAsync();
    Task<LineaProduccionEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(LineaProduccionEntity linea);
    Task UpdateAsync(LineaProduccionEntity linea);
    Task DeleteAsync(Guid id);
}
