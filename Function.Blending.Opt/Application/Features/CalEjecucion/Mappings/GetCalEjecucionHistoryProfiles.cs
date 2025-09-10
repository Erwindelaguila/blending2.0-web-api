using AutoMapper;
using Function.Blending.Opt.Application.Features.CalEjecucion.DTOs.Responses;
using CalEjecucionHistoryItemDomain = Function.Blending.Opt.Domain.ReadModels.CalEjecucionHistoryItemRm;

namespace Function.Blending.Opt.Application.Features.CalEjecucion.Mappings;

public sealed class GetCalEjecucionHistoryProfiles : Profile
{
  public GetCalEjecucionHistoryProfiles()
  {
    CreateMap<CalEjecucionHistoryItemDomain, CalEjecucionHistoryItemResponse>();
  }
}
