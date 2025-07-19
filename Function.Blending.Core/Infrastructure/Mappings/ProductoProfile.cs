using AutoMapper;
using Calidad = Function.Blending.Core.Infrastructure.Persistence.Models.Calidad;
using Producto = Function.Blending.Core.Infrastructure.Persistence.Models.Producto;
using TipoProduccion = Function.Blending.Core.Infrastructure.Persistence.Models.TipoProduccion;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class ProductoProfile : Profile
{
    public ProductoProfile()
    {
        CreateMap<Producto, Domain.Entities.ProductoEntity>()
            .ForMember(dest => dest.Calidad, opt => opt.MapFrom(src => src.Calidad))
            .ForMember(dest => dest.TipoProduccion, opt => opt.MapFrom(src => src.TipoProduccion))
            .ReverseMap();

        CreateMap<Calidad, Domain.Entities.CalidadEntity>().ReverseMap();
        CreateMap<TipoProduccion, Domain.Entities.TipoProduccionEntity>().ReverseMap();
    }
}