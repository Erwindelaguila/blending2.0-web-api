using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Common.Commands;
using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class CreateParametroCommand : BaseCommand<ParametroDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public bool? Activo { get; }

    [JsonConstructor]
    public CreateParametroCommand(
        string codigo,
        string nombre,
        string? descripcion = null,
        bool? activo = null)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = activo ?? true;
    }
}
