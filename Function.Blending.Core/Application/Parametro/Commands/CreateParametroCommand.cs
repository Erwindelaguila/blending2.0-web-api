using Function.Blending.Core.Application.Parametro.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class CreateParametroCommand : IRequest<ParametroDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public bool? Activo { get; }
    public Guid CreadoPorId { get; }

    public CreateParametroCommand(
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
