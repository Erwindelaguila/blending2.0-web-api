using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using Dto = Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses.Input;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class CalidadInputVoToDtoProfile : Profile
{
  public CalidadInputVoToDtoProfile()
  {
    CreateMap<VO.CalInpFiltro, Dto.CalInpFiltroDto>()
      .ForMember(d => d.FechaCorte, o => o.MapFrom(s => s.FechaCorteUtc));
  }
}
