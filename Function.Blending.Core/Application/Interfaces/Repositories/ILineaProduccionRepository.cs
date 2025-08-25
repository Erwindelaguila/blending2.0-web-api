using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ILineaProduccionRepository
{
    Task<List<LineaProduccionEntity>> GetAllAsync();
    Task<LineaProduccionEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    IQueryable<LineaProduccionEntity> GetQueryable();
    Task<(IReadOnlyList<LineaProduccionEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(LineaProduccionEntity linea);
    Task UpdateAsync(LineaProduccionEntity linea);
    Task<LineaProduccionEntity> UpdateAndReturnAsync(LineaProduccionEntity linea);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
    Task<List<LineaProduccionActivaDTO>> GetActivasAsync();
    Task<int> GetActiveTipoProduccionCountAsync(Guid lineaProduccionId);
    Task<bool> IsUsedByActiveTipoProduccionAsync(Guid lineaProduccionId);
}
