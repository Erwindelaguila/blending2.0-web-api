using Function.Blending.Core.Application.Common.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

/// <summary>
/// Command para actualizar un tipo de producción
/// Hereda de BaseCommand para mantener el contexto necesario para autenticación
/// </summary>
public class UpdateTipoProduccionCommand : BaseCommand<TipoProduccionDTO>
{
    public Guid Id { get; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public Guid LineaProduccionId { get; }
    public Guid AgregadoId { get; }
    public bool? Activo { get; }

    public UpdateTipoProduccionCommand(
        Guid id,
        string codigo,
        string nombre,
        string? descripcion,
        Guid lineaProduccionId,
        Guid agregadoId,
        bool? activo,
        object requestContext) : base(requestContext)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        LineaProduccionId = lineaProduccionId;
        AgregadoId = agregadoId;
        Activo = activo;
    }
}
