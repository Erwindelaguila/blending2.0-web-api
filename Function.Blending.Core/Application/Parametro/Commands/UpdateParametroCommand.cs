using Function.Blending.Core.Application.Parametro.DTOs;
using MediatR;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class UpdateParametroCommand : IRequest<ParametroDTO>
{
    public Guid Id { get; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public bool? Activo { get; }
    public Guid ModificadoPorId { get; }

    public UpdateParametroCommand(
        Guid id,
        string codigo,
        string nombre,
        string? descripcion,
        bool? activo,
        Guid modificadoPorId)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = activo;
        ModificadoPorId = modificadoPorId;
    }
}
