using AutoMapper;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class CalidadProfile : Profile
{
    public CalidadProfile()
    {
        CreateMap<Calidad, CalidadEntity>().ReverseMap();
    }
}