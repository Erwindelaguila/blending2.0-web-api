
using AutoMapper;
using Producto = Function.Blending.Core.Infrastructure.Persistence.Models.Producto;
using Calidad = Function.Blending.Core.Infrastructure.Persistence.Models.Calidad;
using TipoProduccionModel = Function.Blending.Core.Infrastructure.Persistence.Models.TipoProduccion;
using Function.Blending.Core.Application.Producto.DTOs;
using Function.Blending.Core.Application.TipoProduccion.DTOs;
using Function.Blending.Core.Domain.Entities;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class ProductoProfile : Profile
{
    public ProductoProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio, incluyendo navegación
        CreateMap<Producto, ProductoEntity>()
            .ForMember(dest => dest.Calidad, opt => opt.MapFrom(src => src.Calidad))
            .ForMember(dest => dest.TipoProduccion, opt => opt.MapFrom(src => src.TipoProduccion))
            .ReverseMap();

        // Relaciones
        CreateMap<Calidad, CalidadEntity>().ReverseMap();
        CreateMap<TipoProduccionModel, TipoProduccionEntity>().ReverseMap();

        // Mapeo entre entidad de dominio y DTOs
        CreateMap<ProductoEntity, ProductoDTO>();
        CreateMap<CalidadEntity, CalidadDTO>();
        CreateMap<TipoProduccionEntity, TipoProduccionDTO>();
    }
}
