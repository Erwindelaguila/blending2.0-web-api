using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using DTO = Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Input;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Mappings
{
  public sealed class GetLogEjecucionInputByIdProfiles : Profile
  {
    public GetLogEjecucionInputByIdProfiles()
    {
      // VO -> DTO
      CreateMap<VO.LogInpInfo, DTO.LogInpInfoDto>();
      CreateMap<VO.LogInpDemanda, DTO.LogInpDemandaDto>();
      CreateMap<VO.LogInpDemParametro, DTO.LogInpDemParametroDto>();
      CreateMap<VO.LogInpOferta, DTO.LogInpOfertaDto>();
      CreateMap<VO.LogInpOfeParametro, DTO.LogInpOfeParametroDto>();
      CreateMap<VO.LogInpOfeOtros, DTO.LogInpOfeOtrosDto>();
      CreateMap<VO.LogInpFiltro, DTO.LogInpFiltroDto>();
      CreateMap<VO.LogInpFilCapacidad, DTO.LogInpFilCapacidadDto>();
      CreateMap<VO.LogInpFilDivision, DTO.LogInpFilDivisionDto>();
      CreateMap<VO.LogInpFilEmparejamiento, DTO.LogInpFilEmparejamientoDto>();
    }
  }
}
