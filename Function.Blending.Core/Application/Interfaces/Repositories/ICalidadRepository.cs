using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ICalidadRepository
{
    Task CreateAsync(CalidadEntity calidadEntity);
    Task<List<CalidadEntity>> GetAllAsync();
    Task UpdateAsync(CalidadEntity calidadEntity);
    Task<CalidadEntity?> GetByIdAsync(Guid id);
}