using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Application.Producto.DTOs;

namespace Function.Blending.Core.Application.Interfaces.Repositories;

public interface IProductoRepository
{
    Task<List<ProductoEntity>> GetAllAsync();
    Task<List<ProductoDTO>> GetAllWithRelationsAsync();
    Task<ProductoEntity?> GetByIdAsync(Guid id);
    Task<ProductoDTO?> GetByIdWithRelationsAsync(Guid id);
    Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null);
    IQueryable<ProductoEntity> GetQueryable();
    IQueryable<Function.Blending.Core.Infrastructure.Persistence.Models.Producto> GetEntityQueryable();
    Task CreateAsync(ProductoEntity producto);
    Task UpdateAsync(ProductoEntity producto);
    Task<ProductoEntity> UpdateAndReturnAsync(ProductoEntity producto);
    Task DeleteAsync(Guid id, Guid eliminadoPorId);
}