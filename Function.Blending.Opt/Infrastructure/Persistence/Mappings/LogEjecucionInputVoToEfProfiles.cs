using AutoMapper;
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;
using Vo = Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Persistence.Mappings
{
  public sealed class LogEjecucionInputVoToEfProfiles : Profile
  {
    public LogEjecucionInputVoToEfProfiles()
    {
      // === Info (1:1 con Ejecucion) ===
      CreateMap<Vo.LogInpInfo, Ef.LogInpInfo>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
        .ForMember(d => d.Ejecucion, o => o.Ignore()); // navegación

      // === Filtro (1:1 con Ejecucion) ===
      CreateMap<Vo.LogInpFiltro, Ef.LogInpFiltro>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
        .ForMember(d => d.Ejecucion, o => o.Ignore()) // navegación
                                                      // colecciones: se insertan con el inserter; evitar doble mapeo
        .ForMember(d => d.LogInpFilCapacidad, o => o.Ignore())
        .ForMember(d => d.LogInpFilDivision, o => o.Ignore())
        .ForMember(d => d.LogInpFilEmparejamiento, o => o.Ignore());

      // === Hijos de Filtro ===
      CreateMap<Vo.LogInpFilCapacidad, Ef.LogInpFilCapacidad>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.FiltroId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetParentId()))
        .ForMember(d => d.Filtro, o => o.Ignore()); // navegación

      CreateMap<Vo.LogInpFilDivision, Ef.LogInpFilDivision>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.FiltroId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetParentId()))
        .ForMember(d => d.Filtro, o => o.Ignore()); // navegación

      CreateMap<Vo.LogInpFilEmparejamiento, Ef.LogInpFilEmparejamiento>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.FiltroId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetParentId()))
        .ForMember(d => d.ParametroId, o => o.MapFrom(s => (Guid)s.ParametroId)) // VO Id tipado -> Guid
        .ForMember(d => d.Filtro, o => o.Ignore())     // navegación
        .ForMember(d => d.Parametro, o => o.Ignore());    // navegación, si existe en tu EF

      // === Oferta (1:1 con Ejecucion) ===
      CreateMap<Vo.LogInpOferta, Ef.LogInpOferta>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.EjecucionId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetExecutionId()))
        .ForMember(d => d.Ejecucion, o => o.Ignore()) // navegación
        .ForMember(d => d.LogInpOfeParametro, o => o.Ignore()); // hijos se insertan aparte

      // === Hijos de Oferta ===
      CreateMap<Vo.LogInpOfeParametro, Ef.LogInpOfeParametro>()
        .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
        .ForMember(d => d.OfertaId, o => o.MapFrom((_, __, ___, ctx) => ctx.GetParentId()))
        .ForMember(d => d.Oferta, o => o.Ignore()); // navegación
    }
  }
}
