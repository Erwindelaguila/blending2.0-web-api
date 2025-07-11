using AutoMapper;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Persistence.Mappings;

public class PlantaProfile : Profile
{
    public PlantaProfile()
    {
        CreateMap<Plantum, Planta>().ReverseMap();
    }
}