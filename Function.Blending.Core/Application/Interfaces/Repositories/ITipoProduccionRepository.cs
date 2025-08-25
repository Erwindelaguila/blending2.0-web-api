using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ITipoProduccionRepository
{
    Task<List<TipoProduccionEntity>> GetAllAsync();
    Task<List<TipoProduccionDTO>> GetAllWithRelationsAsync();
    Task<TipoProduccionEntity?> GetByIdAsync(Guid id);
    Task<TipoProduccionDTO?> GetByIdWithRelationsAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    IQueryable<TipoProduccionEntity> GetQueryable();
    Task<(IReadOnlyList<TipoProduccionEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(TipoProduccionEntity tipo);
    Task UpdateAsync(TipoProduccionEntity tipo);
    Task<TipoProduccionEntity> UpdateAndReturnAsync(TipoProduccionEntity tipo);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
    Task<bool> IsUsedByActiveProductoAsync(Guid tipoProduccionId);
    Task<List<TipoProduccionActivaDTO>> GetActivasAsync();
}
