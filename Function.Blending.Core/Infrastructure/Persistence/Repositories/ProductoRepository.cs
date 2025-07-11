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

    public async Task<List<Domain.Entities.Producto>> GetAllAsync()
    {
        return await _context.Productos
            .Include(p => p.Calidad)
            .Include(p => p.TipoProduccion)
            .AsNoTracking()
            .ProjectTo<Domain.Entities.Producto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Domain.Entities.Producto?> GetByIdAsync(Guid id)
    {
        var model = await _context.Productos.FindAsync(id);
        return model == null ? null : _mapper.Map<Domain.Entities.Producto>(model);
    }

    public async Task AddAsync(Domain.Entities.Producto producto)
    {
        var model = _mapper.Map<Producto>(producto);
        _context.Productos.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.Producto producto)
    {
        var model = await _context.Productos.FindAsync(producto.Id);
        if (model == null) return;

        // Mapear manualmente si quieres evitar sobrescribir CreadoEl/CreadoPorId
        model.Codigo = producto.Codigo;
        model.Nombre = producto.Nombre;
        model.Descripcion = producto.Descripcion;
        model.CalidadId = producto.CalidadId;
        model.TipoProduccionId = producto.TipoProduccionId;
        model.Activo = producto.Activo;
        model.ModificadoPorId = producto.ModificadoPorId;
        model.ModificadoEl = producto.ModificadoEl ?? DateTime.UtcNow;

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
