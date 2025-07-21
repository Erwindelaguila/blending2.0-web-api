using AutoMapper;
using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Domain.Entities;
using Function.Blending.Core.Infrastructure.Persistence.Models;
namespace Function.Blending.Core.Infrastructure.Mappings;

public class AgregadoProfile : Profile
{
    public AgregadoProfile()
    {
        // Mapeo entre modelo de persistencia y entidad de dominio
        CreateMap<Agregado, AgregadoEntity>().ReverseMap();

        // Mapeos para DTOs
        CreateMap<AgregadoEntity, AgregadoDTO>();
    }
}
