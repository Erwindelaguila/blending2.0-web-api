using AutoMapper;
using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Persistence.Mappings;

public class ParametroProfile : Profile
{
    public ParametroProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio
        CreateMap<Parametro, ParametroEntity>().ReverseMap();
        
        // Mapeos para DTOs
        CreateMap<ParametroEntity, ParametroDTO>();
            
     
    }
}
