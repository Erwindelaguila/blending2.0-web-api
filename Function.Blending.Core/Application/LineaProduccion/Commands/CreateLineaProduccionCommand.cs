using MediatR;
using Function.Blending.Core.Application.LineaProduccion.DTOs;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class CreateLineaProduccionCommand : IRequest<LineaProduccionDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public bool? Activo { get; }
    public Guid CreadoPorId { get; }

    public CreateLineaProduccionCommand(string codigo, string nombre, string? descripcion, bool? activo, Guid creadoPorId)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = activo;
        CreadoPorId = creadoPorId;
    }
}
