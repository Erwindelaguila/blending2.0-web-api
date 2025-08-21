using MediatR;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

public class UpdateTipoProduccionCommand : IRequest<TipoProduccionDTO>
{
    public Guid Id { get; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public Guid LineaProduccionId { get; }
    public Guid AgregadoId { get; }
    public bool? Activo { get; }
    public Guid ModificadoPorId { get; }

    public UpdateTipoProduccionCommand(
        Guid id,
        string codigo,
        string nombre,
        string? descripcion,
        Guid lineaProduccionId,
        Guid agregadoId,
        bool? activo,
        Guid modificadoPorId)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        LineaProduccionId = lineaProduccionId;
        AgregadoId = agregadoId;
        Activo = activo;
        ModificadoPorId = modificadoPorId;
    }
}
