using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Services;
using Function.Blending.Core.Application.Menu.DTOs;
using Function.Blending.Core.Application.Menu.DTOs.Graph;

namespace Function.Blending.Core.Infrastructure.Mappings
{
    /// <summary>
    /// Profile de AutoMapper para mapear entre DTOs de Microsoft Graph y modelos de aplicación.
    /// Siguiendo Clean Architecture, centraliza toda la lógica de mapeo.
    /// </summary>
    public class GraphProfile : Profile
    {
        public GraphProfile()
        {
            // Mapeo de respuesta de Microsoft Graph a modelo de aplicación
            CreateMap<GraphUserResponse, GraphUserInfo>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.GivenName, opt => opt.MapFrom(src => src.GivenName))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
                .ForMember(dest => dest.Mail, opt => opt.MapFrom(src => src.Mail))
                .ForMember(dest => dest.UserPrincipalName, opt => opt.MapFrom(src => src.UserPrincipalName));
        }
    }
}
