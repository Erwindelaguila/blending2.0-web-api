using AutoMapper;
using Function.Blending.Opt.Domain.ValueObjects.Ids;
using System;

namespace Function.Blending.Opt.Application.Common.Mappings;

public sealed class StrongIdConvertersProfile : Profile
{
  public StrongIdConvertersProfile()
  {
    CreateMap<EstadoId, Guid>().ConvertUsing(s => s.Value);
    CreateMap<PlantaId, Guid>().ConvertUsing(s => s.Value);
    CreateMap<EjecucionId, Guid>().ConvertUsing(s => s.Value);
    CreateMap<CalidadId, Guid>().ConvertUsing(s => s.Value);
    CreateMap<ParametroId, Guid>().ConvertUsing(s => s.Value);
  }
}
