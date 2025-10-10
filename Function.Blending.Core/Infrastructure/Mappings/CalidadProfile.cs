using AutoMapper;
using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class CalidadProfile : Profile
{
    public CalidadProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio
        CreateMap<Calidad, CalidadEntity>().ReverseMap();
        
        // Mapeos para DTOs
        CreateMap<CalidadEntity, CalidadDTO>();
    }
}