using Function.Blending.Core.Application.Agregado.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Commands
{
    public class CreateAgregadoCommand : IRequest<AgregadoDTO>
    {
        public string Codigo { get; }
        public string Nombre { get; }
        public string? Descripcion { get; }
        public bool? Activo { get; }
        public Guid CreadoPorId { get; }

        public CreateAgregadoCommand(
            string codigo,
           string nombre,
            string? descripcion,
            bool? activo,
            Guid creadoPorId)
        {
            Codigo = codigo;
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = activo;
            CreadoPorId = creadoPorId;
        }
    }
}
