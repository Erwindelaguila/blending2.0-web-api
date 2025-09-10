using AutoMapper;
using Function.Blending.Opt.Application.Features.LogEjecucion.DTOs.Responses;
using LogEjecucionHistoryItemDomain = Function.Blending.Opt.Domain.ReadModels.LogEjecucionHistoryItemRm;

namespace Function.Blending.Opt.Application.Features.LogEjecucion.Mappings;

public sealed class GetLogEjecucionHistoryProfiles : Profile
{
  public GetLogEjecucionHistoryProfiles()
  {
    CreateMap<LogEjecucionHistoryItemDomain, LogEjecucionHistoryItemResponse>();
  }
}
