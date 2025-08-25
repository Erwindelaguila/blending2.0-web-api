using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Application.Calidad.DTOs;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ICalidadRepository
{
    Task<List<CalidadEntity>> GetAllAsync();
    Task<CalidadEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    IQueryable<CalidadEntity> GetQueryable();
    Task<(IReadOnlyList<CalidadEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(CalidadEntity calidad);
    Task UpdateAsync(CalidadEntity calidad);
    Task<CalidadEntity> UpdateAndReturnAsync(CalidadEntity calidad);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
    Task<bool> IsUsedByActiveProductoAsync(Guid calidadId);
    Task<List<CalidadActivaDTO>> GetActivasAsync();
}