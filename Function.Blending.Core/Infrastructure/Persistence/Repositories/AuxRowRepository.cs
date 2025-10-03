using AutoMapper;
using Function.Blending.Core.Application.AuxRow.DTOs;
using Function.Blending.Core.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class AuxRowRepository : IAuxRowRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;
    
    public AuxRowRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<StatusQualityDTO>> GetStatusQualityAsync(Guid id)
    {
        return await _context.AuxRow
            .Where(c => c.TableId == id)
            .Select(c => new StatusQualityDTO
            {
                Id = c.Id,
                Nombre = c.Nombre
            })
            .ToListAsync();
    }
    
}