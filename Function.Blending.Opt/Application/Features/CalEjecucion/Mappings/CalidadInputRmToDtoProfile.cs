using AutoMapper;
using RM = Function.Blending.Opt.Domain.ReadModels;
using Dto = Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Input;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Common;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class CalidadInputRmToDtoProfile : Profile
{
  public CalidadInputRmToDtoProfile()
  {
    CreateMap<RM.CalInpParametroItemRm, Dto.CalInpParametroDto>()
      .ForMember(d => d.Calidad, o => o.MapFrom(s => new BasicRefDto() { Id = s.CalidadId, Codigo = s.CalidadCodigo, Nombre = s.CalidadNombre}))
      .ForMember(d => d.Parametro, o => o.MapFrom(s => new BasicRefDto() { Id = s.ParametroId, Codigo = s.ParametroCodigo, Nombre = s.ParametroNombre}))
      .ForMember(d => d.Valor, o => o.MapFrom(s => s.Valor));
  }
}
