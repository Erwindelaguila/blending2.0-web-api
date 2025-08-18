using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ICalidadRepository
{
    Task<List<CalidadEntity>> GetAllAsync();
    Task<CalidadEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    Task<(IReadOnlyList<CalidadEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(CalidadEntity calidad);
    Task UpdateAsync(CalidadEntity calidad);
    Task<CalidadEntity> UpdateAndReturnAsync(CalidadEntity calidad);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}