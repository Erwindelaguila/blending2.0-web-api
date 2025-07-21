using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IProductoRepository
{
    Task<List<ProductoEntity>> GetAllAsync();
    Task<ProductoEntity?> GetByIdAsync(Guid id);
    Task CreateAsync(ProductoEntity producto);
    Task UpdateAsync(ProductoEntity producto);
    Task DeleteAsync(Guid id);
}