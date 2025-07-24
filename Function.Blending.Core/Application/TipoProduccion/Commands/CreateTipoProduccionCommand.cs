using MediatR;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

public class CreateTipoProduccionCommand : IRequest<TipoProduccionDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public Guid LineaProduccionId { get; }
    public Guid AgregadoId { get; }
    public bool Activo { get; }
    public Guid CreadoPorId { get; }

    public CreateTipoProduccionCommand(
        string codigo,
        string nombre,
        string? descripcion,
        Guid lineaProduccionId,
        Guid agregadoId,
        bool activo,
        Guid creadoPorId)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        LineaProduccionId = lineaProduccionId;
        AgregadoId = agregadoId;
        Activo = activo;
        CreadoPorId = creadoPorId;
    }
}
