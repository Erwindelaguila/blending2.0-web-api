using AutoMapper;
using Function.Blending.Core.Application.AppParam.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;

namespace Function.Blending.Core.Infrastructure.Persistence.Mappings;

public class AppParamProfile : Profile
{
    public AppParamProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio
        CreateMap<AppParam, AppParamEntity>().ReverseMap();
        
        // Mapeos para DTOs
        CreateMap<AppParamEntity, AppParamDTO>();
    }
}
