using AutoMapper;
using VO = Function.Blending.Opt.Domain.ValueObjects;
using DTO = Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses.Output;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Mappings
{
  public sealed class GetLogEjecucionOututByIdProfiles : Profile
  {
    public GetLogEjecucionOututByIdProfiles()
    {
      // VO -> DTO
      CreateMap<VO.LogOutContenedor, DTO.LogOutContenedorDto>();
      CreateMap<VO.LogOutConDistribucion, DTO.LogOutConDistribucionDto>();
      CreateMap<VO.LogOutConComposicion, DTO.LogOutConComposicionDto>();
    }
  }
}
