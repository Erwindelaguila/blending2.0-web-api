using AutoMapper;
using Function.Blending.Opt.Domain.ValueObjects.Ids;
using Dto = Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Requests.Input;
using Vo = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Mappings
{
  public sealed class LogisticaInputDtoToVoProfile : Profile
  {
    public LogisticaInputDtoToVoProfile()
    {
      // Elementos
      CreateMap<Dto.LogInpFilCapacidadDto, Vo.LogInpFilCapacidad>();
      CreateMap<Dto.LogInpFilDivisionDto, Vo.LogInpFilDivision>();

      // Caso especial: ID tipado (Guid -> ParametroId), como en Calidad
      CreateMap<Dto.LogInpFilEmparejamientoDto, Vo.LogInpFilEmparejamiento>()
        .ConstructUsing(s => new Vo.LogInpFilEmparejamiento(
          s.Grupo,
          ParametroId.From(s.ParametroId),
          s.Valor));

      CreateMap<Dto.LogInpOfeParametroDto, Vo.LogInpOfeParametro>();

      // Raíces (constructor mapping simple — no hay renombres aquí)
      CreateMap<Dto.LogInpInfoDto, Vo.LogInpInfo>();
      CreateMap<Dto.LogInpFiltroDto, Vo.LogInpFiltro>();
      CreateMap<Dto.LogInpOfertaDto, Vo.LogInpOferta>();
    }
  }
}
