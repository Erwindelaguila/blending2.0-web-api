using Function.Blending.Core.Application.Calidad.DTOs;
using Function.Blending.Core.Application.Common.Commands;
using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Calidad.Commands;

public class CreateCalidadCommand : BaseCommand<CalidadDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string CodigoMaterial { get; }
    public string? Descripcion { get; }
    public bool? NoConforme { get; }
    public bool? Activo { get; }

    [JsonConstructor]
    public CreateCalidadCommand(
        string codigo,
        string nombre,
        string codigoMaterial,
        string? descripcion = null,
        bool? noConforme = null,
        bool? activo = null)
    {
        Codigo = codigo;
        Nombre = nombre;
        CodigoMaterial = codigoMaterial;
        Descripcion = descripcion;
        NoConforme = noConforme;
        Activo = activo ?? true;
    }
}