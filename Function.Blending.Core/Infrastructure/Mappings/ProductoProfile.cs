using AutoMapper;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
using Calidad = Function.Blending.Core.Infrastructure.Persistence.Models.Calidad;
using Producto = Function.Blending.Core.Infrastructure.Persistence.Models.Producto;
using TipoProduccion = Function.Blending.Core.Infrastructure.Persistence.Models.TipoProduccion;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class ProductoProfile : Profile
{
    public ProductoProfile()
    {
        CreateMap<Producto, Domain.Entities.Producto>()
            .ForMember(dest => dest.Calidad, opt => opt.MapFrom(src => src.Calidad))
            .ForMember(dest => dest.TipoProduccion, opt => opt.MapFrom(src => src.TipoProduccion))
            .ReverseMap();

        CreateMap<Calidad, Domain.Entities.Calidad>().ReverseMap();
        CreateMap<TipoProduccion, Domain.Entities.TipoProduccion>().ReverseMap();
    }
}