using AutoMapper;
using AutoMapper.QueryableExtensions;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Producto = Function.Blending.Core.Infrastructure.Persistence.Models.Producto;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;

    public ProductoRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductoEntity>> GetAllAsync()
    {
        return await _context.Productos
            .Include(p => p.Calidad)
            .Include(p => p.TipoProduccion)
            .AsNoTracking()
            .ProjectTo<ProductoEntity>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ProductoEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Productos.FindAsync(id);
        return model == null ? null : _mapper.Map<Domain.Entities.ProductoEntity>(model);
    }

    public async Task CreateAsync(ProductoEntity productoEntity)
    {
        var model = _mapper.Map<Producto>(productoEntity);
        _context.Productos.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.ProductoEntity productoEntity)
    {
        var model = await _context.Productos.FindAsync(productoEntity.Id);
        if (model == null) return;

        // Mapear manualmente si quieres evitar sobrescribir CreadoEl/CreadoPorId
        model.Codigo = productoEntity.Codigo;
        model.Nombre = productoEntity.Nombre;
        model.Descripcion = productoEntity.Descripcion;
        model.CalidadId = productoEntity.CalidadId;
        model.TipoProduccionId = productoEntity.TipoProduccionId;
        model.Activo = productoEntity.Activo;
        model.ModificadoPorId = productoEntity.ModificadoPorId;
        model.ModificadoEl = productoEntity.ModificadoEl ?? DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var model = await _context.Productos.FindAsync(id);
        if (model == null) return;

        _context.Productos.Remove(model);
        await _context.SaveChangesAsync();
    }
    
}
