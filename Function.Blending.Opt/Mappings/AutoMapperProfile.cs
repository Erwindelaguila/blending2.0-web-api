using AutoMapper;

using DomainEntity = Function.Blending.Opt.Domain.Entities.ModelExecution;
using EfEntity = Function.Blending.Opt.Infrastructure.Data.Scaffolded.ModelExecution;

namespace Function.Blending.Opt.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<DomainEntity, EfEntity>().ReverseMap();
    }
}
