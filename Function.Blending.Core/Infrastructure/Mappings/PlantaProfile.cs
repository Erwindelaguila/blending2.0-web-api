using AutoMapper;
using Function.Blending.Core.Application.Planta.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Persistence.Mappings;

public class PlantaProfile : Profile
{
    public PlantaProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio
        CreateMap<Planta, PlantaEntity>().ReverseMap();
        
        // Mapeos para DTOs
        CreateMap<PlantaEntity, PlantaDTO>();
            
        CreateMap<CreatePlantaDTO, PlantaEntity>();
        CreateMap<UpdatePlantaDTO, PlantaEntity>();
    }
}