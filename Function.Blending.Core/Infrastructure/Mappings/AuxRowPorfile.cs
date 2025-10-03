using AutoMapper;

namespace Function.Blending.Core.Infrastructure.Mappings;

public class AuxRowPorfile : Profile
{
    public AuxRowPorfile()
    {
        CreateMap<AuxRowPorfile, AuxRowPorfile>().ReverseMap();
    }
}