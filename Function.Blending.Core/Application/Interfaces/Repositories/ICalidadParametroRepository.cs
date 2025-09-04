using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface ICalidadParametroRepository
{
    Task<List<CalidadParametroEntity>> GetAllAsync();
    Task<CalidadParametroEntity?> GetByIdAsync(Guid id);
    Task<CalidadParametroEntity?> GetByCalidadParametroAsync(Guid calidadId, Guid parametroId);
    Task<List<CalidadParametroEntity>> GetMatrizDataAsync();
    Task CreateAsync(CalidadParametroEntity calidadParametroEntity);
    Task UpdateAsync(CalidadParametroEntity calidadParametroEntity);
    Task UpsertAsync(Guid calidadId, Guid parametroId, decimal valor, Guid userId);
    Task<int> UpsertBatchAsync(List<(Guid CalidadId, Guid ParametroId, decimal Valor)> cambios, Guid userId);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}
