using Function.Blending.Core.Application.Common.Commands;
using Function.Blending.Core.Application.TipoProduccion.DTOs;

namespace Function.Blending.Core.Application.TipoProduccion.Commands;

/// <summary>
/// Command para crear un tipo de producción
/// Hereda de BaseCommand para mantener el contexto necesario para autenticación
/// </summary>
public class CreateTipoProduccionCommand : BaseCommand<TipoProduccionDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public Guid LineaProduccionId { get; }
    public Guid AgregadoId { get; }
    public bool? Activo { get; }

    public CreateTipoProduccionCommand(
        string codigo,
        string nombre,
        string? descripcion,
        Guid lineaProduccionId,
        Guid agregadoId,
        bool? activo,
        object requestContext) : base(requestContext)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        LineaProduccionId = lineaProduccionId;
        AgregadoId = agregadoId;
        Activo = activo;
    }
}
