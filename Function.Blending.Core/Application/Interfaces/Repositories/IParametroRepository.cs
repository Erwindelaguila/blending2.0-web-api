using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IParametroRepository
{
    Task<List<ParametroEntity>> GetAllAsync();
    Task<ParametroEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    Task<(IReadOnlyList<ParametroEntity> Items, int Total)> GetPagedAsync(int page, int size);
    Task CreateAsync(ParametroEntity parametro);
    Task UpdateAsync(ParametroEntity parametro);
    Task<ParametroEntity> UpdateAndReturnAsync(ParametroEntity parametro);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}
