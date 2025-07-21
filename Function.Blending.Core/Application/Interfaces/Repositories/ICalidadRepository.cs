using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ICalidadRepository
{
    Task<List<CalidadEntity>> GetAllAsync();
    Task<CalidadEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(CalidadEntity calidad);
   
    Task UpdateAsync(CalidadEntity calidad);
    Task DeleteAsync(Guid id);
}