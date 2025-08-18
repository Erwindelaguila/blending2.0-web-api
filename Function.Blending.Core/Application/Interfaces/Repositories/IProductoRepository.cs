using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IProductoRepository
{
    Task<List<ProductoEntity>> GetAllAsync();
    Task<ProductoEntity?> GetByIdAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    IQueryable<ProductoEntity> GetQueryable();
    Task CreateAsync(ProductoEntity producto);
    Task UpdateAsync(ProductoEntity producto);
    Task<ProductoEntity> UpdateAndReturnAsync(ProductoEntity producto);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}