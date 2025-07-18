using MediatR;
using Function.Blending.Core.Application.LineaProduccion.DTOs;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class UpdateLineaProduccionCommand : IRequest<LineaProduccionDTO>
{
    public Guid Id { get; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public bool? Activo { get; }
    public Guid? ModificadoPorId { get; }

    public UpdateLineaProduccionCommand(Guid id, string codigo, string nombre, string? descripcion, bool? activo, Guid? modificadoPorId)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = activo;
        ModificadoPorId = modificadoPorId;
    }
}
