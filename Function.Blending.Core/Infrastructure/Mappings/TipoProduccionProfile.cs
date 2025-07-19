using AutoMapper;
using Agregado = Function.Blending.Core.Infrastructure.Persistence.Models.Agregado;
using LineaProduccion = Function.Blending.Core.Infrastructure.Persistence.Models.LineaProduccion;
using TipoProduccionModel = Function.Blending.Core.Infrastructure.Persistence.Models.TipoProduccion;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class TipoProduccionProfile : Profile
{
    public TipoProduccionProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio, incluyendo navegación
        CreateMap<TipoProduccionModel, TipoProduccionEntity>()
            .ForMember(dest => dest.Agregado, opt => opt.MapFrom(src => src.Agregado))
            .ForMember(dest => dest.LineaProduccion, opt => opt.MapFrom(src => src.LineaProduccion))
            .ReverseMap();

        CreateMap<Agregado, AgregadoEntity>().ReverseMap();
        CreateMap<LineaProduccion, LineaProduccionEntity>().ReverseMap();

        // Mapeo entre entidad de dominio y DTOs
        CreateMap<TipoProduccionEntity, TipoProduccionDTO>();
        CreateMap<CreateTipoProduccionDTO, TipoProduccionEntity>();
        CreateMap<UpdateTipoProduccionDTO, TipoProduccionEntity>();
    }
}
