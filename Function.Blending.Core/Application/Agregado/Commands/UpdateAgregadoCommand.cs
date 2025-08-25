using Function.Blending.Core.Application.Agregado.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Agregado.Commands
{
    public class UpdateAgregadoCommand : IRequest<AgregadoDTO>
    {
        public Guid Id { get; }
        public string Codigo { get; }
        public string Nombre { get; }
        public string? Descripcion { get; }
        public bool? Activo { get; }
        // Hacer nullable para evitar error de deserialización cuando llega null
        public Guid? ModificadoPorId { get; }

        public UpdateAgregadoCommand(
            Guid id,
            string codigo,
            string nombre,
            string? descripcion,
            bool? activo,
            Guid? modificadoPorId)
        {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = activo;
            ModificadoPorId = modificadoPorId;
        }
    }
}
