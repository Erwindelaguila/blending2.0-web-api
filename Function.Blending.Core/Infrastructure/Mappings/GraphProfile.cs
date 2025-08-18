using AutoMapper;
using Function.Blending.Core.Application.Interfaces.Services;

namespace Function.Blending.Core.Infrastructure.Mappings
{
    /// <summary>
    /// Profile de AutoMapper para mapear entre DTOs y modelos de aplicación.
    /// Siguiendo Clean Architecture, centraliza toda la lógica de mapeo.
    /// </summary>
    public class GraphProfile : Profile
    {
        public GraphProfile()
        {
            // Mapeos para entidades del Core se configuran aquí
            // Por ahora está vacío ya que las funciones de Menu se movieron a Function.Blending.Auth
        }
    }
}
