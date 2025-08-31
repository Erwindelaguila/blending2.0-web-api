using Function.Blending.Core.Application.Parametro.DTOs;
using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;
using System.Text.Json.Serialization;

namespace Function.Blending.Core.Application.Parametro.Commands;

public class UpdateParametroCommand : BaseCommand<ParametroDTO>
{
    public Guid Id { get; }
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public bool? Activo { get; }

    [JsonConstructor]
    public UpdateParametroCommand(
        Guid id,
        string codigo,
        string nombre,
        string? descripcion = null,
        bool? activo = null,
        HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = activo;
    }
}
