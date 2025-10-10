using AutoMapper;
using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class LineaProduccionProfile : Profile
{
    public LineaProduccionProfile()
    {
        CreateMap<LineaProduccion, LineaProduccionEntity>().ReverseMap();

        CreateMap<LineaProduccionEntity, LineaProduccionDTO>();
    }
}
