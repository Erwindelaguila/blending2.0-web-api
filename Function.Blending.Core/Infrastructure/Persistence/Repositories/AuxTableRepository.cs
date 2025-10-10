using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Repositories;

namespace Function.Blending.Core.Infrastructure.Persistence.Repositories;

public class AuxTableRepository : IAuxTableRepository
{
    private readonly BlendingDbContext _context;
    private readonly IMapper _mapper;
    
    public AuxTableRepository(BlendingDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
}