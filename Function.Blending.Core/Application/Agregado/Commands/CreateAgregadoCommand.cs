using Function.Blending.Core.Application.Agregado.DTOs;
using Function.Blending.Core.Application.Common.Commands;

namespace Function.Blending.Core.Application.Agregado.Commands
{
    public class CreateAgregadoCommand : BaseCommand<AgregadoDTO>
    {
        public string Codigo { get; }
        public string Nombre { get; }
        public string? Descripcion { get; }
        public bool? Activo { get; }

        public CreateAgregadoCommand(
            string codigo,
            string nombre,
            string? descripcion,
            bool? activo,
            object requestContext) : base(requestContext)
        {
            Codigo = codigo;
            Nombre = nombre;
            Descripcion = descripcion;
            Activo = activo;
        }
    }
}
