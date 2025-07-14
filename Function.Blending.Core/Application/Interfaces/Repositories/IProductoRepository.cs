using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IProductoRepository
{
    Task<List<ProductoEntity>> GetAllAsync();
    Task<ProductoEntity?> GetByIdAsync(Guid id);
    Task AddAsync(ProductoEntity productoEntity);
    Task UpdateAsync(ProductoEntity productoEntity);
    Task DeleteAsync(Guid id);
}