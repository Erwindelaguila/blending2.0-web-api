using Function.Blending.Core.Application.LineaProduccion.DTOs;
using Function.Blending.Core.Application.Common.Commands;
using Microsoft.Azure.Functions.Worker.Http;

namespace Function.Blending.Core.Application.LineaProduccion.Commands;

public class CreateLineaProduccionCommand : BaseCommand<LineaProduccionDTO>
{
    public string Codigo { get; }
    public string Nombre { get; }
    public string? Descripcion { get; }
    public bool? Activo { get; }

    public CreateLineaProduccionCommand(
        string codigo,
        string nombre,
        string? descripcion = null,
        bool? activo = null,
        HttpRequestData? requestContext = null) : base(requestContext!)
    {
        Codigo = codigo;
        Nombre = nombre;
        Descripcion = descripcion;
        Activo = activo ?? true;
    }
}
