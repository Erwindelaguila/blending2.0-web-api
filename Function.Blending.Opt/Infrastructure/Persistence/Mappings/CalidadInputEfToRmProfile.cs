using AutoMapper;
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;
using RM = Function.Blending.Opt.Domain.ReadModels;
using Ids = Function.Blending.Opt.Domain.ValueObjects.Ids;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings;

public sealed class CalidadInputEfToRmProfile : Profile
{
  public CalidadInputEfToRmProfile()
  {
    CreateMap<Ef.CalInpParametro, RM.CalInpParametroItemRm>()
      .ForMember(d => d.CalidadCodigo, opt => opt.MapFrom(s => s.Calidad != null ? s.Calidad.Codigo : null))
      .ForMember(d => d.CalidadNombre, opt => opt.MapFrom(s => s.Calidad != null ? s.Calidad.Nombre: null))
      .ForMember(d => d.ParametroCodigo, opt => opt.MapFrom(s => s.Parametro != null ? s.Parametro.Codigo : null))
      .ForMember(d => d.ParametroNombre, opt => opt.MapFrom(s => s.Parametro != null ? s.Parametro.Nombre: null))
      .ForMember(d => d.Valor, opt => opt.MapFrom(s => s.Valor));
  }
}
