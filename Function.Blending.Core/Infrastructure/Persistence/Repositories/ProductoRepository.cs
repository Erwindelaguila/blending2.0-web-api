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
        return await _context.Producto
            .Where(x => !x.Eliminado)
            .Include(p => p.Calidad)
            .Include(p => p.TipoProduccion)
            .AsNoTracking()
            .ProjectTo<ProductoEntity>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ProductoEntity?> GetByIdAsync(Guid id)
    {
        var model = await _context.Producto
            .Where(x => x.Id == id && !x.Eliminado)
            .FirstOrDefaultAsync();
        return model == null ? null : _mapper.Map<Domain.Entities.ProductoEntity>(model);
    }

    public async Task<bool> ExistsActiveCodigoAsync(string codigo, Guid? excludeId = null)
    {
        var query = _context.Producto.Where(x => x.Codigo == codigo && !x.Eliminado);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public IQueryable<ProductoEntity> GetQueryable()
    {
        return _context.Producto
            .Where(x => !x.Eliminado)
            .Select(p => new ProductoEntity
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                CalidadId = p.CalidadId,
                TipoProduccionId = p.TipoProduccionId,
                Activo = p.Activo,
                CreadoPorId = p.CreadoPorId,
                CreadoEl = p.CreadoEl,
                ModificadoPorId = p.ModificadoPorId,
                ModificadoEl = p.ModificadoEl,
                Eliminado = p.Eliminado,
                EliminadoPorId = p.EliminadoPorId,
                EliminadoEl = p.EliminadoEl
            });
    }

    public async Task CreateAsync(ProductoEntity productoEntity)
    {
        var model = _mapper.Map<Producto>(productoEntity);
        _context.Producto.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.ProductoEntity productoEntity)
    {
        var model = await _context.Producto.FindAsync(productoEntity.Id);
        if (model == null || model.Eliminado) return;

        // Mapear manualmente si quieres evitar sobrescribir CreadoEl/CreadoPorId
        model.Codigo = productoEntity.Codigo;
        model.Nombre = productoEntity.Nombre;
        model.Descripcion = productoEntity.Descripcion ?? string.Empty;
        model.CalidadId = productoEntity.CalidadId;
        model.TipoProduccionId = productoEntity.TipoProduccionId;
        model.Activo = productoEntity.Activo;
        model.ModificadoPorId = productoEntity.ModificadoPorId;
        model.ModificadoEl = productoEntity.ModificadoEl ?? DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<ProductoEntity> UpdateAndReturnAsync(ProductoEntity productoEntity)
    {
        var model = await _context.Producto.FindAsync(productoEntity.Id);
        if (model == null || model.Eliminado)
            throw new InvalidOperationException("Producto no encontrado o eliminado");

        // Actualizar propiedades
        model.Codigo = productoEntity.Codigo;
        model.Nombre = productoEntity.Nombre;
        model.Descripcion = productoEntity.Descripcion ?? string.Empty;
        model.CalidadId = productoEntity.CalidadId;
        model.TipoProduccionId = productoEntity.TipoProduccionId;
        model.Activo = productoEntity.Activo;
        model.ModificadoPorId = productoEntity.ModificadoPorId;
        model.ModificadoEl = productoEntity.ModificadoEl ?? DateTime.UtcNow;

        _context.Producto.Update(model);
        await _context.SaveChangesAsync();

        // Retornar la entidad actualizada
        return _mapper.Map<ProductoEntity>(model);
    }

    public async Task DeleteAsync(Guid id, Guid eliminadoPorId)
    {
        var entity = await _context.Producto.FindAsync(id);
        if (entity is null || entity.Eliminado) return;

        // Soft delete
        entity.Eliminado = true;
        entity.EliminadoPorId = eliminadoPorId;
        entity.EliminadoEl = DateTime.UtcNow;

        _context.Producto.Update(entity);
        await _context.SaveChangesAsync();
    }
}
