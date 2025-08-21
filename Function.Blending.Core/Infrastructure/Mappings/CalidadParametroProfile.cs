using AutoMapper;
using Function.Blending.Core.Application.CalidadParametro.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class CalidadParametroProfile : Profile
{
    public CalidadParametroProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio
        CreateMap<CalidadParametro, CalidadParametroEntity>()
            .ReverseMap();

        // Mapeo entre entidad de dominio y DTO
        CreateMap<CalidadParametroEntity, CalidadParametroDTO>()
            .ReverseMap();
    }
}
