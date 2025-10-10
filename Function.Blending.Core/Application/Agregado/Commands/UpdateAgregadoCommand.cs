using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.Agregado.Commands
{
   
    public class UpdateAgregadoCommand : BaseCommand<AgregadoDTO>
    {
        public Guid Id { get; }
        public string Codigo { get; }
        public string Nombre { get; }
        public string? Descripcion { get; }
        public bool? Activo { get; }

        public UpdateAgregadoCommand(
            Guid id,
            string codigo,
            string nombre,
            string? descripcion,
            bool? activo)
        {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = activo;
        }
    }
}
